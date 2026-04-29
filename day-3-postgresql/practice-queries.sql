-- ============================================================
--  SECTION 1: JOINS (BASIC TO INTERMEDIATE)
-- ============================================================



-- Q1. List all customers and the total number of orders they
--     have placed. Show only customers with more than 5 orders.
--     Sort by total orders descending.

select companyname, count(*) as order_count
from customers c join orders o
on c.customerid = o.customerid
group by c.customerid
having count(*) > 5
order by order_count desc;


-- ------------------------------------------------------------

-- Q2. Retrieve the total sales amount per customer by joining
--     customers, orders and order_details. Show only customers
--     whose total sales exceed 10,000. Sort by total sales
--     descending.

select c.companyname, c.contactname,
       sum(od.unitprice * od.quantity * (1 - od.discount)) as total_sales
from customers c
    join orders o on c.customerid = o.customerid
    join order_details od on od.orderid = o.orderid
group by c.customerid
having sum(od.unitprice * od.quantity * (1 - od.discount)) > 10000
order by total_sales desc;


-- ------------------------------------------------------------

-- Q3. Get the number of products per category. Show only
--     categories having more than 10 products. Sort by product
--     count descending.

select categoryname, count(productid) as product_count
from products p join categories c
on p.categoryid = c.categoryid
group by c.categoryid
having count(productid) > 10
order by product_count desc;


-- ------------------------------------------------------------

-- Q4. Display the total quantity sold per product. Include only
--     products where total quantity sold is greater than 100.
--     Sort by quantity descending.

select productname, sum(od.quantity) as total_quantity
from products p join order_details od
on p.productid = od.productid
group by p.productid
having sum(od.quantity) > 100
order by total_quantity desc;


-- ------------------------------------------------------------

-- Q5. Find the total number of orders handled by each employee.
--     Show only employees who handled more than 20 orders.
--     Sort by order count descending.

select concat(firstname, ' ', lastname) as employee_name,
       count(orderid) as total_orders
from employees e join orders o
on e.employeeid = o.employeeid
group by e.employeeid
having count(orderid) > 20
order by total_orders desc;


-- ============================================================
--  SECTION 2: JOINS (INTERMEDIATE)
-- ============================================================

-- Q6. Retrieve the total sales per category by joining
--     categories, products and order_details. Show only
--     categories with total sales above 50,000. Sort by total
--     sales descending.

select c.categoryname,
       sum(od.unitprice * od.quantity * (1 - od.discount)) as total_sales
from categories c
    join products p on c.categoryid = p.categoryid
    join order_details od on p.productid = od.productid
group by c.categoryid, c.categoryname
having sum(od.unitprice * od.quantity * (1 - od.discount)) > 50000
order by total_sales desc;


-- ------------------------------------------------------------

-- Q7. List suppliers and the number of products they supply.
--     Show only suppliers who supply more than 5 products.
--     Sort by product count descending.

select s.companyname, count(p.productid) as product_count
from suppliers s join products p
on s.supplierid = p.supplierid
group by s.supplierid, s.companyname
having count(p.productid) > 5
order by product_count desc;


-- ------------------------------------------------------------

-- Q8. Get the average unit price per category. Show only
--     categories where the average price is above 30. Sort by
--     average price descending.

select c.categoryname, avg(p.unitprice) as avg_price
from categories c join products p
on c.categoryid = p.categoryid
group by c.categoryid, c.categoryname
having avg(p.unitprice) > 30
order by avg_price desc;


-- ------------------------------------------------------------

-- Q9. Display the total revenue generated per employee. Show
--     only employees generating more than 20,000 in revenue.
--     Sort by revenue descending.

select concat(e.firstname, ' ', e.lastname) as employee_name,
       sum(od.unitprice * od.quantity * (1 - od.discount)) as total_revenue
from employees e
    join orders o on e.employeeid = o.employeeid
    join order_details od on o.orderid = od.orderid
group by e.employeeid, e.firstname, e.lastname
having sum(od.unitprice * od.quantity * (1 - od.discount)) > 20000
order by total_revenue desc;


-- ------------------------------------------------------------

-- Q10. Retrieve the number of orders shipped to each country.
--      Show only countries with more than 10 orders. Sort by
--      order count descending.

select shipcountry, count(orderid) as total_orders
from orders
group by shipcountry
having count(orderid) > 10
order by total_orders desc;


-- ============================================================
--  SECTION 3: JOINS (ADVANCED)
-- ============================================================

-- Q11. Find customers and the average order value. Show only
--      customers with average order value greater than 500.
--      Sort by average descending.

select companyname, avg(od.unitprice * od.quantity * (1 - od.discount)) as avg_sales
from customers c
    join orders o on o.customerid = c.customerid
    join order_details od on o.orderid = od.orderid
group by c.customerid
having avg(od.unitprice * od.quantity * (1 - od.discount)) > 500
order by avg_sales desc;


-- ------------------------------------------------------------

-- Q12. Get the top-selling products per category by total
--      quantity sold. Show only products with total quantity
--      sold above 200. Sort within category by quantity
--      descending.

select c.categoryname, p.productname, sum(od.quantity) as total_quantity
from products p
    join order_details od on p.productid = od.productid
    join categories c on c.categoryid = p.categoryid
group by p.productname, c.categoryname
having sum(od.quantity) > 200
order by categoryname asc, total_quantity desc;


-- ------------------------------------------------------------

-- Q13. Retrieve the total discount given per product. Show
--      only products where total discount exceeds 1,000.
--      Sort by discount descending.

select p.productname,
       sum(od.unitprice * od.quantity * od.discount) as total_discount
from products p join order_details od
on p.productid = od.productid
group by p.productid, p.productname
having sum(od.unitprice * od.quantity * od.discount) > 1000
order by total_discount desc;


-- ------------------------------------------------------------

-- Q14. List employees and the number of unique customers they
--      handled. Show only employees who handled more than 15
--      unique customers. Sort by count descending.

select concat(e.firstname, ' ', e.lastname) as employee_name,
       count(distinct o.customerid) as unique_customers
from employees e join orders o
on e.employeeid = o.employeeid
group by e.employeeid, e.firstname, e.lastname
having count(distinct o.customerid) > 15
order by unique_customers desc;


-- ------------------------------------------------------------

-- Q15. Find the monthly total sales (year + month). Show only
--      months where total sales exceed 30,000. Sort by year
--      and month ascending.

select extract(year from o.orderdate) as year,
       extract(month from o.orderdate) as month,
       sum(od.unitprice * od.quantity * (1 - od.discount)) as monthly_sales
from orders o join order_details od
on o.orderid = od.orderid
group by extract(year from o.orderdate), extract(month from o.orderdate)
having sum(od.unitprice * od.quantity * (1 - od.discount)) > 30000
order by year asc, month asc;


-- ============================================================
--  SECTION 4: STORED PROCEDURES & TRANSACTIONS
-- ============================================================

-- SP1. Insert a new customer. Rollback if any required value
--      is missing.

create or replace procedure insert_new_customer(
    customerid   character(5),
    companyname  character(40),
    contactname  character(30),
    contacttitle character(30),
    address      character(60),
    city         character(15),
    region       character(15),
    postalcode   character(10),
    country      character(15),
    phone        character(24),
    fax          character(24)
)
language plpgsql
as $$
begin
    if (customerid is null or trim(customerid) = '') then
        raise exception 'customer id missing';
    end if;

    if (companyname is null or trim(companyname) = '') then
        raise exception 'company name missing';
    end if;

    insert into customers(
        customerid, companyname, contactname, contacttitle,
        address, city, region, postalcode, country, phone, fax
    )
    values(
        customerid, companyname, contactname, contacttitle,
        address, city, region, postalcode, country, phone, fax
    );

    raise notice 'customer added';

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call insert_new_customer(
    'NANDK', 'Nandhi Stores', 'Nandhiraja K', 'Sales Manager',
    '12 Anna Nagar', 'Erode', 'Tamil Nadu', '638109',
    'India', '987***3210', '234dd4343'
);

select * from customers where customerid = 'NANDK';


-- ------------------------------------------------------------

-- SP2. Place a new order for an existing customer with one
--      product. Insert into orders and order_details in a
--      single transaction.

create or replace procedure place_order(
    p_customerid     char(5),
    p_employeeid     int,
    p_productid      int,
    p_quantity       smallint,
    p_unitprice      numeric(19,4),
    p_discount       real,
    p_requireddate   timestamp,
    p_shippeddate    timestamp,
    p_shipvia        int,
    p_freight        numeric,
    p_shipname       varchar(40),
    p_shipaddress    varchar(60),
    p_shipcity       varchar(15),
    p_shipregion     varchar(15),
    p_shippostalcode varchar(10),
    p_shipcountry    varchar(15)
)
language plpgsql
as $$
declare
    new_orderid int;
begin
    if not exists (select 1 from customers where customerid = p_customerid) then
        raise exception 'customer missing';
    end if;

    insert into orders(
        customerid, employeeid, orderdate,
        requireddate, shippeddate, shipvia, freight,
        shipname, shipaddress, shipcity,
        shipregion, shippostalcode, shipcountry
    )
    values(
        p_customerid, p_employeeid, current_date,
        p_requireddate, p_shippeddate, p_shipvia, p_freight,
        p_shipname, p_shipaddress, p_shipcity,
        p_shipregion, p_shippostalcode, p_shipcountry
    )
    returning orderid into new_orderid;

    insert into order_details(orderid, productid, unitprice, quantity, discount)
    values(new_orderid, p_productid, p_unitprice, p_quantity, p_discount);

    raise notice 'order placed: %', new_orderid;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call place_order(
    'ALFKI'::char(5),
    1::int,
    11::int,
    5::smallint,
    14.00::numeric(19,4),
    0.05::real,
    (current_date + interval '7 days')::timestamp,
    null::timestamp,
    2::int,
    25.50::numeric,
    'Alfreds Futterkiste'::varchar(40),
    'Obere Str. 57'::varchar(60),
    'Berlin'::varchar(15),
    null::varchar(15),
    '12209'::varchar(10),
    'Germany'::varchar(15)
);

select * from orders where orderid = 11078;


-- ------------------------------------------------------------

-- SP3. Update product stock after an order is placed. Rollback
--      if stock is not enough.

create or replace procedure update_stock(
    p_orderid   int,
    p_productid int,
    p_quantity  smallint
)
language plpgsql
as $$
declare
    v_stock smallint;
begin
    if not exists (select 1 from orders where orderid = p_orderid) then
        raise exception 'order missing';
    end if;

    select unitsinstock into v_stock
    from products where productid = p_productid;

    if v_stock is null then
        raise exception 'product missing';
    end if;

    if v_stock < p_quantity then
        raise exception 'stock low: % available', v_stock;
    end if;

    update products
    set unitsinstock = unitsinstock - p_quantity
    where productid = p_productid;

    raise notice 'stock updated: % left', v_stock - p_quantity;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call update_stock(10248::int, 11::int, 5::smallint);

select * from products where productid = 11;


-- ------------------------------------------------------------

-- SP4. Cancel an order. Delete from order_details first, then
--      from orders, using a transaction.

create or replace procedure cancel_order(
    p_orderid int
)
language plpgsql
as $$
begin
    if not exists (select 1 from orders where orderid = p_orderid) then
        raise exception 'order not found';
    end if;

    delete from order_details where orderid = p_orderid;
    delete from orders where orderid = p_orderid;

    raise notice 'order cancelled';

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call cancel_order(10248::int);


-- ------------------------------------------------------------

-- SP5. Transfer products from one supplier to another. Rollback
--      if either supplier does not exist.

create or replace procedure transfer_supplier(
    p_old_supplierid int,
    p_new_supplierid int
)
language plpgsql
as $$
declare
    v_count int;
begin
    if not exists (select 1 from suppliers where supplierid = p_old_supplierid) then
        raise exception 'old supplier missing';
    end if;

    if not exists (select 1 from suppliers where supplierid = p_new_supplierid) then
        raise exception 'new supplier missing';
    end if;

    select count(*) into v_count
    from products where supplierid = p_old_supplierid;

    if v_count = 0 then
        raise exception 'no products for old supplier';
    end if;

    update products
    set supplierid = p_new_supplierid
    where supplierid = p_old_supplierid;

    raise notice '% product(s) transferred', v_count;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

-- before transfer
select supplierid, count(*) from products group by supplierid having supplierid = 1;
select supplierid, count(*) from products group by supplierid having supplierid = 2;

call transfer_supplier(1::int, 2::int);

-- after transfer
select supplierid, count(*) from products group by supplierid having supplierid = 2;


-- ------------------------------------------------------------

-- SP6. Update the price of all products in a category by a
--      percentage. Rollback if percentage is less than or
--      equal to zero.

create or replace procedure update_price(
    p_categoryid int,
    p_percentage numeric
)
language plpgsql
as $$
declare
    v_count int;
begin
    if p_percentage <= 0 then
        raise exception 'invalid percentage: %', p_percentage;
    end if;

    if not exists (select 1 from categories where categoryid = p_categoryid) then
        raise exception 'category missing';
    end if;

    select count(*) into v_count
    from products where categoryid = p_categoryid;

    if v_count = 0 then
        raise exception 'no products in category';
    end if;

    update products
    set unitprice = unitprice + (unitprice * p_percentage / 100)
    where categoryid = p_categoryid;

    raise notice '% product(s) price updated by % %%', v_count, p_percentage;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call update_price(1::int, 10::numeric);


-- ------------------------------------------------------------

-- SP7. Add a new product under an existing category and
--      supplier. Rollback if category or supplier does not
--      exist.

create or replace procedure add_product(
    p_productname  varchar(40),
    p_supplierid   int,
    p_categoryid   int,
    p_unitprice    numeric(19,4),
    p_unitsinstock smallint,
    p_discontinued smallint default 0
)
language plpgsql
as $$
begin
    if p_productname is null or trim(p_productname) = '' then
        raise exception 'product name missing';
    end if;

    if not exists (select 1 from categories where categoryid = p_categoryid) then
        raise exception 'category missing';
    end if;

    if not exists (select 1 from suppliers where supplierid = p_supplierid) then
        raise exception 'supplier missing';
    end if;

    if p_unitprice <= 0 then
        raise exception 'invalid price';
    end if;

    insert into products(
        productname, supplierid, categoryid,
        unitprice, unitsinstock, discontinued
    )
    values(
        p_productname, p_supplierid, p_categoryid,
        p_unitprice, p_unitsinstock, p_discontinued
    );

    raise notice 'product added: %', p_productname;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call add_product('mango juice'::varchar, 1::int, 1::int, 25.00::numeric(19,4), 100::smallint);


-- ------------------------------------------------------------

-- SP8. Delete a customer only if the customer has no orders.
--      Rollback if orders exist.

create or replace procedure delete_customer(
    p_customerid char(5)
)
language plpgsql
as $$
declare
    v_order_count int;
begin
    if not exists (select 1 from customers where customerid = p_customerid) then
        raise exception 'customer not found';
    end if;

    select count(*) into v_order_count
    from orders where customerid = p_customerid;

    if v_order_count > 0 then
        raise exception 'cannot delete: % order(s) exist', v_order_count;
    end if;

    delete from customers where customerid = p_customerid;

    raise notice 'customer deleted: %', p_customerid;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call delete_customer('NANDK'::char(5));


-- ------------------------------------------------------------

-- SP9. Apply a discount to all order details for a specific
--      order. Rollback if the order does not exist.

create or replace procedure apply_discount(
    p_orderid  int,
    p_discount real
)
language plpgsql
as $$
declare
    v_count int;
begin
    if not exists (select 1 from orders where orderid = p_orderid) then
        raise exception 'order not found';
    end if;

    if p_discount <= 0 or p_discount > 1 then
        raise exception 'invalid discount: must be between 0 and 1';
    end if;

    select count(*) into v_count
    from order_details where orderid = p_orderid;

    if v_count = 0 then
        raise exception 'no items in order';
    end if;

    update order_details
    set discount = p_discount
    where orderid = p_orderid;

    raise notice '% item(s) discount applied: %', v_count, p_discount;

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call apply_discount(10250::int, 0.10::real);


-- ------------------------------------------------------------

-- SP10. Place an order with multiple products. Insert order and
--       all items in one transaction. Rollback if any product
--       is invalid or stock is insufficient.

create or replace procedure multi_order(
    p_customerid char(5),
    p_employeeid int,
    p_productids int[],
    p_quantities smallint[],
    p_unitprices numeric(19,4)[]
)
language plpgsql
as $$
declare
    v_orderid int;
    v_stock   smallint;
    i         int;
begin
    if not exists (select 1 from customers where customerid = p_customerid) then
        raise exception 'customer missing';
    end if;

    if array_length(p_productids, 1) <> array_length(p_quantities, 1) or
       array_length(p_productids, 1) <> array_length(p_unitprices, 1) then
        raise exception 'product, quantity, price count mismatch';
    end if;

    for i in 1..array_length(p_productids, 1) loop
        select unitsinstock into v_stock
        from products where productid = p_productids[i];

        if v_stock is null then
            raise exception 'product % missing', p_productids[i];
        end if;

        if v_stock < p_quantities[i] then
            raise exception 'stock low for product %: % available', p_productids[i], v_stock;
        end if;
    end loop;

    insert into orders(customerid, employeeid, orderdate)
    values(p_customerid, p_employeeid, current_date)
    returning orderid into v_orderid;

    for i in 1..array_length(p_productids, 1) loop
        insert into order_details(orderid, productid, unitprice, quantity, discount)
        values(v_orderid, p_productids[i], p_unitprices[i], p_quantities[i], 0);

        update products
        set unitsinstock = unitsinstock - p_quantities[i]
        where productid = p_productids[i];
    end loop;

    raise notice 'order placed: %, items: %', v_orderid, array_length(p_productids, 1);

exception
    when others then
        raise notice 'rolled back: %', sqlerrm;
        rollback;
end;
$$;

call multi_order(
    'ALFKI'::char(5),
    1::int,
    array[11, 42, 72]::int[],
    array[5, 10, 3]::smallint[],
    array[14.00, 9.80, 34.80]::numeric(19,4)[]
);


