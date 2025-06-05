

-- Cursor-Based Questions (5)
-- 1.Write a cursor that loops through all films and prints titles longer than 120 minutes.

do $$
declare
    film_rec record;
    film_cursor cursor for
        select title, length from film;
begin
    open film_cursor;

    loop
        fetch film_cursor into film_rec;
        exit when not found;

        if film_rec.length > 120 then
            raise notice 'Long film: % (% minutes)', film_rec.title, film_rec.length;
        end if;
    end loop;

    close film_cursor;
end;
$$;

-- 2.Create a cursor that iterates through all customers and counts how many rentals each made.
do $$
declare
    cust_rec record;
    rental_count int;
    cust_cursor cursor for
        select customer_id, first_name, last_name from customer;
begin
    open cust_cursor;

    loop
        fetch cust_cursor into cust_rec;
        exit when not found;

        select count(*) into rental_count
        from rental
        where customer_id = cust_rec.customer_id;

        raise notice 'Customer: % %, ID: %, Rentals: %',
            cust_rec.first_name, cust_rec.last_name,
            cust_rec.customer_id, rental_count;
    end loop;

    close cust_cursor;
end;
$$;

-- 3.Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.

do $$
declare
    film_rec record;
    rental_count int;
    film_cursor cursor for
        select film_id, title, rental_rate from film;
begin
    open film_cursor;

    loop
        fetch film_cursor into film_rec;
        exit when not found;
        
        select count(*) into rental_count
        from inventory i
        join rental r on r.inventory_id = i.inventory_id
        where i.film_id = film_rec.film_id;

        if rental_count < 5 then
            update film
            set rental_rate = rental_rate + 1
            where film_id = film_rec.film_id;

            raise notice 'Increased rental rate for "%": was rented % time(s)',
                film_rec.title, rental_count;
        end if;
    end loop;

    close film_cursor;
end;
$$;

-- 4.Create a function using a cursor that collects titles of all films from a particular category.

create or replace function get_films_by_category(category_name text)
returns table(title text) as $$
declare
    film_cursor cursor for
        select f.title
        from film f
        join film_category fc on f.film_id = fc.film_id
        join category c on fc.category_id = c.category_id
        where c.name = category_name;

    film_rec record;
begin
    open film_cursor;

    loop
        fetch film_cursor into film_rec;
        exit when not found;

        title := film_rec.title;  
        return next;              
    end loop;

    close film_cursor;
end;
$$ language plpgsql;



-- 5.Loop through all stores and count how many distinct films are available in each store using a cursor.

do $$
declare
    store_rec record;
    film_count int;
    store_cursor cursor for
        select store_id from store;
begin
    open store_cursor;

    loop
        fetch store_cursor into store_rec;
        exit when not found;

        select count(distinct film_id)
        into film_count
        from inventory
        where store_id = store_rec.store_id;

        raise notice 'Store ID: %, Distinct Films: %',
            store_rec.store_id, film_count;
    end loop;

    close store_cursor;
end;
$$;

-- Trigger-Based Questions (5)
-- 1.Write a trigger that logs whenever a new customer is inserted.

create table customer_insert_log (
    log_id serial primary key,
    customer_id int,
    full_name text,
    created_at timestamp default current_timestamp
);


create or replace function log_new_customer()
returns trigger as $$
begin
    insert into customer_insert_log (customer_id, full_name)
    values (
        new.customer_id,
        new.first_name || ' ' || new.last_name
    );

    return new;
end;
$$ language plpgsql;


create trigger trg_log_new_customer
after insert on customer
for each row
execute function log_new_customer();


insert into customer (store_id, first_name, last_name, email, address_id, active, create_date)
values (1, 'Test', 'User', 'test.user@example.com', 1, 1, current_timestamp);

select * from customer_insert_log;


-- 2.Create a trigger that prevents inserting a payment of amount 0.
create or replace function prevent_zero_payment()
returns trigger as $$
begin
    if new.amount = 0 then
        raise exception 'Payment amount cannot be zero.';
    end if;
    return new;
end;
$$ language plpgsql;


create trigger trg_prevent_zero_payment
before insert on payment
for each row
execute function prevent_zero_payment();


insert into payment (customer_id, staff_id, rental_id, amount, payment_date)
values (1, 1, 1, 0, current_timestamp);

-- 3.Set up a trigger to automatically set last_update on the film table before update.

create or replace function update_film_last_update()
returns trigger as $$
begin
    new.last_update := current_timestamp;
    return new;
end;
$$ language plpgsql;

create trigger trg_update_film_last_update
before update on film
for each row
execute function update_film_last_update();

-- 4.Create a trigger to log changes in the inventory table (insert/delete).

create table inventory_change_log (
    log_id serial primary key,
    inventory_id int,
    film_id int,
    store_id int,
    action_type text,             -- 'INSERT' or 'DELETE'
    changed_at timestamp default current_timestamp
);


create or replace function log_inventory_changes()
returns trigger as $$
begin
    if tg_op = 'INSERT' then
        insert into inventory_change_log (inventory_id, film_id, store_id, action_type)
        values (new.inventory_id, new.film_id, new.store_id, 'INSERT');
    elsif tg_op = 'DELETE' then
        insert into inventory_change_log (inventory_id, film_id, store_id, action_type)
        values (old.inventory_id, old.film_id, old.store_id, 'DELETE');
    end if;
    return null;  -- not used for logging-only triggers
end;
$$ language plpgsql;


create trigger trg_log_inventory_changes
after insert or delete on inventory
for each row
execute function log_inventory_changes();


-- Insert test
insert into inventory (film_id, store_id, last_update)
values (1, 1, current_timestamp);

-- Delete test
delete from inventory
where inventory_id = (select max(inventory_id) from inventory);

-- 5) write a trigger that ensures a rental can’t be made for a customer who owes more than $50.

select * from film;

create or replace function block_rental()
returns trigger as $$
declare 
    rentalamount numeric := 0;
    paidamount numeric := 0;
    owed numeric;
begin
    select coalesce(sum(f.rental_rate), 0) into rentalamount
    from rental r
    join inventory i on r.inventory_id = i.inventory_id
    join film f on i.film_id = f.film_id
    where r.customer_id = new.customer_id;

    select coalesce(sum(amount), 0) into paidamount
    from payment 
    where customer_id = new.customer_id;

    owed := rentalamount - paidamount;

    if owed > 50 then
        raise notice '%', new.customer_id;
        raise exception 'rental denied: customer % owes $%', new.customer_id, owed;
    end if;

    return new;
end;
$$ language plpgsql;

create trigger trg_block_rent
before insert on rental
for each row
execute function block_rental();

--transaction

-- 1) write a transaction that inserts a customer and an initial rental in one atomic operation.

create or replace procedure insert_customer_and_rental()
language plpgsql
as $$
declare
    new_customer_id int;
begin
    begin
        insert into customer (store_id, first_name, last_name, email, address_id, active, create_date)    
        values (1, 'singam', 'singh', 'rockybhai@example.com', 5, 1, current_timestamp)
        returning customer_id into new_customer_id;

        insert into rental (
            rental_date, inventory_id, customer_id, staff_id
        )
        values (
            current_timestamp, 5000, new_customer_id, 2
        );
    exception when others then
        raise notice 'transaction failed. rolling back.';
        raise;
    end;
end $$;

call insert_customer_and_rental();

-- 2) simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.

do $$
declare
    filmid int;
begin
    update film
    set rental_rate = 8.99
    where film_id = 8
    returning film_id into filmid;

    insert into inventory (film_id, store_id, last_update)
    values (filmid, 9999999, current_timestamp);
exception when others then
    raise notice 'error occurred: %', sqlerrm;
end $$;

-- 3) create a transaction that transfers an inventory item from one store to another.

do $$
declare 
    source_store_id int := 1;
    dest_store_id int := 2;
    filmid int := 8;
begin
    update inventory
    set store_id = dest_store_id
    where store_id = source_store_id and film_id = filmid;
    
    raise notice 'film transferred from store % to store %', source_store_id, dest_store_id;
exception when others then
    raise notice 'error occurred: %', sqlerrm;
end $$;

-- 4) demonstrate savepoint and rollback to savepoint by updating payment amounts, then undoing one.

select * from payment where payment_id = 17503;

begin;
    update payment set amount = 10.00 
    where payment_id = 17503;

    savepoint before_update;

    update payment set amount = 15.00 
    where payment_id = 17503;

    rollback to before_update;
commit;

-- 5) write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.

select max(customer_id) from customer;

do $$
declare
    cusid int := 599;
begin
    delete from payment
    where customer_id = cusid;

    delete from customer
    where customer_id = cusid;

    delete from rental
    where customer_id = cusid;

    raise notice 'customer and related data deleted successfully.';
exception when others then
    raise notice 'error occurred: %', sqlerrm;
end $$;



 
