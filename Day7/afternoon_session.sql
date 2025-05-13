
--cursor

do $$
declare
    rental_record record;
    rental_cursor cursor for
        select r.rental_id, c.first_name, c.last_name, r.rental_date
        from rental r
        join customer c on r.customer_id = c.customer_id
        order by r.rental_id;
begin
    open rental_cursor;

    loop
        fetch rental_cursor into rental_record;
        exit when not found;

        raise notice 'rental id: %, customer: % %, date: %',
                     rental_record.rental_id,
                     rental_record.first_name,
                     rental_record.last_name,
                     rental_record.rental_date;
    end loop;

    close rental_cursor;
end;
$$;



-- Cursors 
-- Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.
create table customer_rental_summary (
customer_id INT,
customer_name TEXT,
rental_count INT
) 

do 
$$
declare 
	rec record;
	cur cursor for 
		select c.customer_id, c.first_name || ' ' || c.last_name AS customer_name,
               COUNT(r.rental_id) AS rental_count
			   from customer c
		join rental r on c.customer_id = r.customer_id
		group by c.customer_id, c.first_name, c.last_name;
begin 
	open cur;
	loop 
		fetch cur into rec;
		exit when not found;

		insert into customer_rental_summary(customer_id, customer_name, rental_count)
		values (rec.customer_id, rec.customer_name, rec.rental_count);
	end loop;
	close cur;
end;
		
$$;

select * from customer_rental_summary;
-- Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.
DO $$
DECLARE
    rec RECORD;
    cur CURSOR FOR
        SELECT f.title, COUNT(r.rental_id) AS rental_count
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON fc.category_id = c.category_id
        JOIN inventory i ON f.film_id = i.film_id
        JOIN rental r ON i.inventory_id = r.inventory_id
        WHERE c.name = 'Comedy'
        GROUP BY f.film_id, f.title
        HAVING COUNT(r.rental_id) > 10;
BEGIN
    OPEN cur;

    LOOP
        FETCH cur INTO rec;
        EXIT WHEN NOT FOUND;

        RAISE NOTICE 'Title: %, Rentals: %', rec.title, rec.rental_count;
    END LOOP;

    CLOSE cur;
END;
$$;
-- Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.
CREATE TABLE store_film_report (
    store_id INT,
    film_count INT
);

DO $$
DECLARE
    rec RECORD;
    cur CURSOR FOR
        SELECT s.store_id
        FROM store s;
    film_total INT;
BEGIN
    OPEN cur;

    LOOP
        FETCH cur INTO rec;
        EXIT WHEN NOT FOUND;

        
        SELECT COUNT(DISTINCT i.film_id)
        INTO film_total
        FROM inventory i
        WHERE i.store_id = rec.store_id;

       
        INSERT INTO store_film_report (store_id, film_count)
        VALUES (rec.store_id, film_total);
    END LOOP;

    CLOSE cur;
END;
$$;

SELECT * FROM store_film_report

-- Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.
CREATE TABLE inactive_customers (
    customer_id INT,
    first_name TEXT,
    last_name TEXT,
    email TEXT,
    last_rental_date TIMESTAMP
);

DO $$
DECLARE
    rec RECORD;
    cur CURSOR FOR
        SELECT c.customer_id, c.first_name, c.last_name, c.email,
               MAX(r.rental_date) AS last_rental_date
        FROM customer c
        LEFT JOIN rental r ON c.customer_id = r.customer_id
        GROUP BY c.customer_id, c.first_name, c.last_name, c.email
        HAVING MAX(r.rental_date) IS NULL OR MAX(r.rental_date) < NOW() - INTERVAL '6 months';
BEGIN
    OPEN cur;

    LOOP
        FETCH cur INTO rec;
        EXIT WHEN NOT FOUND;

        INSERT INTO inactive_customers (customer_id, first_name, last_name, email, last_rental_date)
        VALUES (rec.customer_id, rec.first_name, rec.last_name, rec.email, rec.last_rental_date);
    END LOOP;

    CLOSE cur;
END;
$$;

SELECT * FROM inactive_customers;
-- --------------------------------------------------------------------------
 
-- Transactions 
-- Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.
DO $$
DECLARE
  new_customer_id INT;
  new_rental_id INT;
  
BEGIN

INSERT INTO customer (store_id, first_name, last_name, email, address_id, activebool, create_date)
VALUES (1, 'Mano', 'Sundar', 'manosundar@example.com', 5, true, current_timestamp)
RETURNING customer_id INTO new_customer_id;

INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
VALUES (current_timestamp, 1, new_customer_id, 1)
RETURNING rental_id INTO new_rental_id;


INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (new_customer_id, 1, new_rental_id, 4.99, current_timestamp);

END;
$$;

select * from customer order by store_id;

 
-- Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.
DO $$
BEGIN

    BEGIN
      
        UPDATE rental
        SET staff_id = 1
        WHERE rental_id = 1;

       
        UPDATE rental
        SET staff_id = 2
        WHERE rental_id = 9999999;

    EXCEPTION
        WHEN OTHERS THEN
            
            RAISE NOTICE 'Transaction failed. Changes will be rolled back.';
           
            RAISE;
    END;
END;
$$;

select * from rental order by rental_id;

-- Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.
select * from payment order by payment_id;

BEGIN;

UPDATE payment
SET amount = amount + 5
WHERE payment_id = 17503;

SAVEPOINT before_second_update;

UPDATE payment
SET amount = -100  
WHERE payment_id = 17504;

ROLLBACK TO SAVEPOINT before_second_update;


UPDATE payment
SET amount = amount + 10
WHERE payment_id = 17505;


COMMIT;

-- Perform a transaction that transfers inventory from one store to another (delete + insert) safely.

select * from inventory;

BEGIN;

WITH inv_data AS (
    SELECT film_id
    FROM inventory
    WHERE inventory_id = 100 AND store_id = 1
)

DELETE FROM inventory
WHERE inventory_id = 100 AND store_id = 1;

INSERT INTO inventory (film_id, store_id, last_update)
SELECT film_id, 2, NOW()
FROM inv_data;

COMMIT;
ABORT;

SELECT * FROM rental WHERE inventory_id = 100;


 
-- Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.
BEGIN;


DELETE FROM payment
WHERE customer_id = 1;


DELETE FROM rental
WHERE customer_id = 1;


DELETE FROM customer
WHERE customer_id = 1;

COMMIT;

select * from customer where customer_id =1;
-- ----------------------------------------------------------------------------
 
-- Triggers
-- Create a trigger to prevent inserting payments of zero or negative amount.
CREATE OR REPLACE FUNCTION prevent_invalid_payment()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.amount <= 0 THEN
        RAISE EXCEPTION 'Invalid payment amount: %. Amount must be greater than zero.', NEW.amount;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trg_prevent_zero_negative_payment
BEFORE INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION prevent_invalid_payment();

SELECT * FROM payment

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (2, 1, 1, 10.00, NOW());

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (2, 1, 1, 0.00, NOW());


-- Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.
SELECT * FROM film;

CREATE OR REPLACE FUNCTION update_last_update_column()
RETURNS TRIGGER AS $$
BEGIN
  IF NEW.title IS DISTINCT FROM OLD.title OR
     NEW.rental_rate IS DISTINCT FROM OLD.rental_rate THEN
    NEW.last_update := CURRENT_TIMESTAMP;
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_last_update
BEFORE UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION update_last_update_column();


UPDATE film
SET title = 'New Film Title'
WHERE film_id = 1;


SELECT title, rental_rate, last_update
FROM film
WHERE film_id = 1;

 
-- Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.

CREATE TABLE rental_log (
    log_id SERIAL PRIMARY KEY,
    film_id INT,
    rental_count INT,
    log_message TEXT,
    log_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


CREATE OR REPLACE FUNCTION check_weekly_rental_count()
RETURNS TRIGGER AS $$
DECLARE
    rental_cnt INT;
BEGIN
    
    SELECT COUNT(*) INTO rental_cnt
    FROM rental r
    JOIN inventory i ON r.inventory_id = i.inventory_id
    WHERE i.film_id = (
        SELECT film_id FROM inventory WHERE inventory_id = NEW.inventory_id
    )
    AND r.rental_date >= NOW() - INTERVAL '7 days';

   
    IF rental_cnt > 3 THEN
        INSERT INTO rental_log(film_id, rental_count, log_message)
        VALUES (
            (SELECT film_id FROM inventory WHERE inventory_id = NEW.inventory_id),
            rental_cnt,
            'Film rented more than 3 times this week.'
        );
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_rental_log
AFTER INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION check_weekly_rental_count();

INSERT INTO rental (rental_date, inventory_id, customer_id, return_date, staff_id, last_update)
VALUES
(NOW() - INTERVAL '1 day', 100, 5, NULL, 1, CURRENT_TIMESTAMP),
(NOW() - INTERVAL '2 days', 100, 2, NULL, 1, CURRENT_TIMESTAMP),
(NOW() - INTERVAL '3 days', 100, 3, NULL, 1, CURRENT_TIMESTAMP),
(NOW() - INTERVAL '4 days', 100, 4, NULL, 1, CURRENT_TIMESTAMP);


SELECT * FROM rental;

SELECT * FROM rental_log ORDER BY log_date DESC;


	



