/* 1. Create a stored procedure to encrypt a given text
Task: Write a stored procedure sp_encrypt_text that takes a plain text input (e.g., email or mobile number) and returns an encrypted version using PostgreSQL's pgcrypto extension.
 
Use pgp_sym_encrypt(text, key) from pgcrypto.
 
2. Create a stored procedure to compare two encrypted texts
Task: Write a procedure sp_compare_encrypted that takes two encrypted values and checks if they decrypt to the same plain text.
 
3. Create a stored procedure to partially mask a given text
Task: Write a procedure sp_mask_text that:
 
Shows only the first 2 and last 2 characters of the input string
 
Masks the rest with *
 
E.g., input: 'john.doe@example.com' → output: 'jo***************om'
 
4. Create a procedure to insert into customer with encrypted email and masked name
Task:
 
Call sp_encrypt_text for email
 
Call sp_mask_text for first_name
 
Insert masked and encrypted values into the customer table
 
Use any valid address_id and store_id to satisfy FK constraints.
 
5. Create a procedure to fetch and display masked first_name and decrypted email for all customers
Task:
Write sp_read_customer_masked() that:
 
Loops through all rows
 
Decrypts email
 
Displays customer_id, masked first name, and decrypted email */


CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE OR REPLACE FUNCTION sp_encrypt_text(input_text TEXT, secret_key TEXT)
RETURNS BYTEA AS $$
BEGIN
    RETURN pgp_sym_encrypt(input_text, secret_key);
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION sp_compare_encrypted(encrypted1 BYTEA, encrypted2 BYTEA, secret_key TEXT)
RETURNS BOOLEAN AS $$
DECLARE
    decrypted1 TEXT;
    decrypted2 TEXT;
BEGIN
    decrypted1 := pgp_sym_decrypt(encrypted1, secret_key);
    decrypted2 := pgp_sym_decrypt(encrypted2, secret_key);
    RETURN decrypted1 = decrypted2;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION sp_mask_text(input_text TEXT)
RETURNS TEXT AS $$
DECLARE
    len INT := LENGTH(input_text);
BEGIN
    IF len <= 4 THEN
        RETURN input_text;
    END IF;
    RETURN SUBSTRING(input_text, 1, 2) || REPEAT('*', len - 4) || SUBSTRING(input_text, len - 1, 2);
END;
$$ LANGUAGE plpgsql;

CREATE TABLE customer_data (
    customer_id SERIAL PRIMARY KEY,
    store_id INT,
    first_name TEXT,
    email BYTEA,
    address_id INT
);

CREATE OR REPLACE PROCEDURE sp_insert_customer(
    p_first_name TEXT,
    p_email TEXT,
    p_store_id INT,
    p_address_id INT,
    p_secret_key TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    masked_name TEXT;
    encrypted_email BYTEA;
BEGIN
    masked_name := sp_mask_text(p_first_name);
    encrypted_email := sp_encrypt_text(p_email, p_secret_key);
    
    INSERT INTO customer_data (store_id, first_name, email, address_id)
    VALUES (p_store_id, masked_name, encrypted_email, p_address_id);
END;
$$;


CREATE OR REPLACE PROCEDURE sp_read_customer_masked(p_secret_key TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
    rec RECORD;
    decrypted_email TEXT;
BEGIN
    FOR rec IN SELECT customer_id, first_name, email FROM customer_data
    LOOP
        decrypted_email := pgp_sym_decrypt(rec.email, p_secret_key);
        RAISE NOTICE 'Customer ID: %, Name: %, Email: %', rec.customer_id, rec.first_name, decrypted_email;
    END LOOP;
END;
$$;



CALL sp_insert_customer('Johnathan', 'john.doe@example.com', 1, 101, 'mysecretkey');
SELECT * FROM customer_data;
CALL sp_read_customer_masked('mysecretkey');





-- md5 - basic algorithm
-- scram-sha-256 - adavanced algorithm
-- peer - linux
-- cert 
-- trust 


-- ROLE/ACCESS

-- create role readonly login password 'ro123';

-- select datname hash_database_priveledge('readonly',datname,'CONNECT') AS canconnect from pg_database;

-- revoke connect on database dbsample from public;
 
-- select datname hash_database_priveledge('readonly',datname,'CONNECT') AS canconnect from pg_database;

-- grant connect on database dbsample to readonly;





