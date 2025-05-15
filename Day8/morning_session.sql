create or replace procedure proc_create_customer_rental_payment(
p_first_name text,p_last_name text, p_email text,p_address_id int, 
p_inventory_id int, p_store_is int,
p_staff_id int,p_amount numeric
)
Language plpgsql
as $$
DECLARE
    v_customer_id INT;
    v_rental_id INT;
BEGIN
  Begin
    INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)
    VALUES (p_store_is,p_first_name,p_last_name,p_email,p_address_id, 1, CURRENT_DATE)
    RETURNING customer_id INTO v_customer_id;
 
    INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
    VALUES (CURRENT_TIMESTAMP, p_inventory_id, v_customer_id, p_staff_id)
    RETURNING rental_id INTO v_rental_id;
    
    INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
    VALUES (v_customer_id, p_staff_id, 100000, p_amount, CURRENT_TIMESTAMP);
  Exception when others then
    raise notice 'Transaction failed %',sqlerrm;
  End;
END; 
$$;

select * from customer order by customer_id  desc

call proc_create_customer_rental_payment ('Ram','Som','ram_som@gmail.com',1,1,1,1,-10)

--High availability - minimize the down time
--Redudencdy - another server - read only server
--Automatic recovery

/* 
Non Available
    - Crash
    - Network Failure
    - Harware Failure
    - Maintenance Time

Master and Slave
    - Master - primary server
    - Slave  - standby

Master -> Crashed/Network/HardwareFailure -> Slave will take over (promotion)




initdb -D "D:/pri"
initdb -D "D:/sec"


pg_ctl -D D:\pri -o "-p 5433" -l d:\pri\logfile start


>psql -p 5433 -d postgres -c "CREATE ROLE replicator with REPLICATION LOGIN PASSWORD 'repl_pass';"

pg_basebackup -D d:\sec -Fp -Xs -P -R -h 127.0.0.1 -U replicator -p 5433

pg_ctl -D D:\sec -o "-p 5435" -l d:\sec\logfile start

psql -p 5433 -U postgres -

(In another cmd)

psql -p 5435 -U postgres

--------------------------------------
5433 - 
select * from pg_stat_replication;
5435
select pg_is_in_recovery();
-------------------------------------
Create table in primary
CREATE TABLE sample_table (
testdb(#     id SERIAL PRIMARY KEY,
testdb(#     name TEXT NOT NULL
testdb(# );
CREATE TABLE
testdb=# INSERT INTO sample_table (name) VALUES ('Hello from primary');
INSERT 0 1

Check in secondary
SELECT * FROM sample_table;
 id |        name
----+--------------------
  1 | Hello from primary
*/  



