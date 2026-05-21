# Stored Procedures in PostgreSQL
## Notes


## 1. What is a Stored Procedure?

A **Stored Procedure** is a saved block of SQL code stored inside the database.
Instead of writing the same SQL again and again, you write it once, save it
as a procedure, and just CALL it whenever needed.

Think of it like a **function in programming** — but it lives inside the database.

```
Without SP:                         With SP:
----------------------------        ----------------------------
INSERT INTO customers ...           CALL insert_customer(...)
check if null...
raise error if missing...
INSERT INTO customers ...
```

> ✅ One call does everything — clean, simple, reusable.

---

## 2. Why Use Stored Procedures?

| Benefit          | What It Means                                           |
|------------------|---------------------------------------------------------|
| Reusability      | Write once, call many times from anywhere               |
| Security         | Hide table structure — expose only the procedure        |
| Performance      | Precompiled — runs faster than raw SQL each time        |
| Maintainability  | Fix logic in one place — all callers get the fix        |
| Transaction Safe | Wrap multiple operations — rollback if anything fails   |
| Less Network     | Send one CALL instead of 10 SQL statements              |

---

## 3. Basic Syntax

```sql
create or replace procedure procedure_name(
    param1  datatype,
    param2  datatype
)
language plpgsql
as $$
declare
    -- local variables here
begin
    -- SQL logic here

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;
```

### Breaking It Down Line by Line

```
create or replace procedure     → create new (or update if exists)
procedure_name(...)             → name of the procedure + input params
language plpgsql                → PostgreSQL procedural language
as $$                           → start of procedure body
declare                         → section for local variables
begin                           → start of logic
exception when others then      → catches any error
rollback                        → undo all changes on error
end;                            → end of procedure
$$;                             → close the body
```

---

## 4. Key Parts Explained

### `create or replace`
- `create` → makes a new procedure
- `or replace` → overwrites it if it already exists (safe to re-run)

### `language plpgsql`
- PostgreSQL's procedural language
- Supports variables, loops, conditions, exception handling

### `$$` Dollar Quoting
- `$$` is used instead of single quotes to wrap the procedure body
- Avoids conflicts with single quotes inside the SQL code

### `begin ... end`
- Everything between `begin` and `end` is your logic
- Must always be present

---

## 5. Parameters

Parameters are inputs you pass when calling the procedure.

```sql
create or replace procedure add_product(
    p_productname  varchar(40),   -- input: product name
    p_unitprice    numeric(19,4), -- input: price
    p_categoryid   int            -- input: category
)
```

### Naming Convention
Always prefix parameters with `p_` so they don't clash with column names:

```sql
-- ❌ Bad — confusing, same name as column
procedure insert_customer(customerid char(5))

-- ✅ Good — clear it's a parameter
procedure insert_customer(p_customerid char(5))
```

### Common Data Types

| Type             | Use For                        |
|------------------|--------------------------------|
| `int`            | IDs, counts                    |
| `smallint`       | Small numbers (e.g. quantity)  |
| `numeric(19,4)`  | Prices, money                  |
| `varchar(n)`     | Variable text                  |
| `char(n)`        | Fixed-length text (e.g. IDs)   |
| `real`           | Discount (0.0 to 1.0)          |
| `timestamp`      | Date + time                    |
| `int[]`          | Array of integers              |
| `smallint[]`     | Array of small integers        |

---

## 6. Variables (DECLARE)

Local variables store temporary values inside the procedure.

```sql
declare
    v_stock    smallint;   -- stores current stock value
    v_count    int;        -- stores a count
    v_orderid  int;        -- stores a newly created order ID
```

### Naming Convention
Always prefix variables with `v_` so they're clearly local variables.

### Assigning a Value

```sql
-- Option 1: direct assignment
v_count := 10;

-- Option 2: from a SELECT
select unitsinstock into v_stock
from products where productid = p_productid;
```

---

## 7. IF / ELSE Conditions

Used to validate inputs and raise errors before doing any DB operation.

```sql
-- simple check
if p_customerid is null or trim(p_customerid) = '' then
    raise exception 'customer id missing';
end if;

-- check if a row exists
if not exists (select 1 from customers where customerid = p_customerid) then
    raise exception 'customer not found';
end if;

-- check a value
if p_percentage <= 0 then
    raise exception 'invalid percentage: %', p_percentage;
end if;

-- if / else
if v_stock >= p_quantity then
    -- enough stock
else
    raise exception 'stock low';
end if;
```

> ✅ Always validate FIRST, then do INSERT/UPDATE/DELETE.

---

## 8. Exception Handling & Rollback

This is the most important part for transactions.

```sql
exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
```

### How It Works

```
begin
  step 1: insert into orders     ✅
  step 2: insert into details    ✅
  step 3: update stock           ❌ ERROR here

  → exception block fires
  → rollback undoes step 1 and step 2
  → database is clean, nothing was saved
```

### `sqlerrm`
- Built-in PostgreSQL variable
- Holds the actual error message text
- Always include it in your notice so you know what went wrong

```sql
-- ❌ Vague
raise notice 'rolled back';

-- ✅ Tells you WHY it rolled back
raise notice 'rolled back: %', sqlerrm;
```

### `when others then`
- Catches ALL types of errors (null, constraint, etc.)
- Can be more specific:

```sql
exception
    when not_null_violation then
        raise notice 'null value error';
    when foreign_key_violation then
        raise notice 'invalid reference';
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
```

---

## 9. Loops

Used in SP10 (multi_order) to loop through arrays of products.

```sql
-- basic loop with counter
for i in 1..array_length(p_productids, 1) loop

    -- access array element
    select unitsinstock into v_stock
    from products where productid = p_productids[i];

    -- do something for each item
    insert into order_details(...)
    values(v_orderid, p_productids[i], ...);

end loop;
```

### Array Parameters

```sql
-- declare array params
p_productids  int[],
p_quantities  smallint[]

-- call with array
call multi_order(
    'ALFKI',
    array[11, 42, 72]::int[],
    array[5, 10, 3]::smallint[]
);

-- access in loop
p_productids[i]    -- element at position i
array_length(p_productids, 1)  -- length of array
```

---

## 10. RAISE NOTICE vs RAISE EXCEPTION

| Command           | What It Does                          | Stops Execution? |
|-------------------|---------------------------------------|------------------|
| `raise notice`    | Prints a message — like console.log   | ❌ No            |
| `raise exception` | Throws an error — triggers rollback   | ✅ Yes           |

```sql
-- prints info — continues running
raise notice 'order placed: %', v_orderid;

-- throws error — jumps to exception block
raise exception 'customer missing';

-- with a value in the message
raise notice 'stock updated: % left', v_stock - p_quantity;
raise notice '% item(s) discount applied: %', v_count, p_discount;
```

### `%` Placeholder
Works like printf — each `%` is replaced by the next argument:

```sql
raise notice '% items updated at % %%', v_count, p_percentage;
-- output: 12 items updated at 10 %

-- %% = literal percent sign
-- %  = next argument value
```

---

## 11. CALL a Procedure

```sql
-- basic call
call procedure_name(value1, value2);

-- with type casting (recommended for PostgreSQL)
call update_stock(10248::int, 11::int, 5::smallint);

-- with named values (readable)
call add_product(
    'mango juice'::varchar,
    1::int,
    1::int,
    25.00::numeric(19,4),
    100::smallint
);

-- with null values — must cast null too
call place_order(
    'ALFKI'::char(5),
    null::timestamp,     -- shippeddate is optional
    null::varchar(15)    -- shipregion is optional
);
```

---

## 12. DROP a Procedure

```sql
-- drop without knowing param types
drop procedure procedure_name;

-- drop with param types (safe when overloaded)
drop procedure update_stock(int, int, smallint);
```

---


## 13. Quick Reference Cheatsheet

```sql
-- CREATE
create or replace procedure sp_name(p_id int, p_name varchar)
language plpgsql as $$

-- DECLARE VARIABLES
declare
    v_count int;
    v_stock smallint;

-- LOGIC
begin
    -- VALIDATE
    if p_id is null then raise exception 'id missing'; end if;
    if not exists (select 1 from table where id = p_id) then
        raise exception 'not found';
    end if;

    -- SELECT INTO variable
    select count(*) into v_count from table where col = p_id;

    -- DML
    insert into table(...) values(...);
    update table set col = val where id = p_id;
    delete from table where id = p_id;

    -- SUCCESS MESSAGE
    raise notice 'done: %', v_count;

-- ERROR HANDLING
exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

-- CALL
call sp_name(1::int, 'value'::varchar);

-- DROP
drop procedure sp_name;
```

---

