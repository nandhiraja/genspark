-- 1. List all customers from USA.
--    Hint: Customers, WHERE

select companyname, country
from customers 
where  country = 'USA'


-- 2. List all products where UnitPrice is greater than 20.
--    Hint: Products, WHERE

   
select *
from products 
where  unitprice >20

-- 3. List all orders placed after 1997-01-01.
--    Hint: Orders, WHERE

select *
from orders 
where  orderdate > '1997-01-01'

-- 4. Display customers ordered by Country and then CompanyName.
--    Hint: Customers, ORDER BY


select * 
from customers
order by country , companyname

-- 5. List products ordered by highest UnitPrice first.
--    Hint: Products, ORDER BY


select * 
from products
order by unitprice desc    -- desc used for show highest price first


-- groupby
-- 6. Count how many customers are there in each country.
--    Hint: Customers, GROUP BY

select country , count(*) as customer_count
from customers
group by country

-- 7. Find the number of products in each category.
--    Hint: Products, GROUP BY

select categoryid , count(*) count_of_products
from products 
group by categoryid
 
   -- to get category name
   select categoryname,count(*)  as count_of_products
   from products p  join categories c
   on p.categoryid  = c.categoryid
   group by c.categoryid
   

-- 8. Find the total number of orders handled by each employee.
--    Hint: Orders, GROUP BY

   
select employeeid , count(*) as orders_count 
from orders
group by employeeid


-- 9. Find the average freight amount for each customer.
--    Hint: Orders, GROUP BY

   
select customerid ,  AVG(freight) as avg_freight_amount
from orders 
group by customerid


-- 10. Find the maximum unit price in each category.
--     Hint: Products, GROUP BY

select categoryid, max(unitprice)
from products
group by categoryid 



--   Having
-- 11. Show countries having more than 5 customers.
--     Hint: Customers, GROUP BY, HAVING
 
select country , count(*) as customers_count
from customers
group by country
having count(*)>5

-- 12. Show employees who handled more than 50 orders.
--     Hint: Orders, GROUP BY, HAVING


select employeeid , count(*) as total_orders
from  orders 
group by employeeid 
having count(*)>50
order by total_orders


-- 13. Show customers whose average freight is greater than 50.
--       Hint: Orders, GROUP BY, HAVING

select customerid , AVG(freight) as avg_freight_value
from orders
group by customerid
having AVG(freight)>50




-- 14. Show categories where the average product price is greater than 30.
--     Hint: Products, GROUP BY, HAVING
select categoryid, AVG(unitprice) as average_price
from products
group by categoryid
having AVG(unitprice) > 30;



-- 15. Show ship countries having more than 20 orders.
--     Hint: Orders, GROUP BY, HAVING

select shipcountry ,  count(*) as orders_count
from orders
group by shipcountry
having count(*)>20
 

 -- Joins

-- 16. List each order with customer company name.
--     Hint: Orders, Customers, JOIN

select orderid ,companyname
from orders o join customers c
on o.customerid  =  c.customerid



-- 17. List each order with employee first name and last name.
--     Hint: Orders, Employees, JOIN

select orderid , firstname ,lastname
from employees e join orders o 
on e.employeeid = o.employeeid


-- 18. List products with their category name.
--     Hint: Products, Categories, JOIN


select productname , categoryname
from products p  join categories c
on p.categoryid =  c.categoryid


-- 19. List products with supplier company name.
--     Hint: Products, Suppliers, JOIN

select productname , companyname as supplier_company
from products p join suppliers s
on p.supplierid  =  s.supplierid


-- 20. List orders with shipper company name.
--     Hint: Orders, Shippers, JOIN

select orderid , companyname as Shipper_company
from orders o  join shippers s
on o.shipvia =  s.shipperid


 -- Medium

-- 21. Find total orders per customer and display customer company name.
--     Hint: Customers, Orders, JOIN, GROUP BY



select count(*) as total_orders , companyname 
from customers c join orders o
on c.customerid = o.customerid
group by c.customerid




-- 22. Find total products supplied by each supplier.
--     Hint: Suppliers, Products, JOIN, GROUP BY

select companyname as supplier_name ,count(*) as total_products
from suppliers s join products p
on s.supplierid  = p.supplierid
group by s.supplierid




-- 23. Find average product price per category with category name.
--     Hint: Categories, Products, JOIN, GROUP BY

select categoryname , AVG(unitprice) as Average_price 
from categories c  join products p
on c.categoryid  =  p.categoryid
group by c.categoryname




-- 24. Find total freight per customer and order by highest total freight.
--     Hint: Customers, Orders, JOIN, GROUP BY, ORDER BY

select companyname ,SUM(freight) as total_freight   
from  customers c join orders o
on  c.customerid  = o.customerid
group by c.customerid
order by total_freight desc




-- 25. Find employees who handled more than 25 orders.
--     Hint: Employees, Orders, JOIN, GROUP BY, HAVING

select  firstname , lastname , count(*) as orders_handle_count
from employees e  join  orders o
on e.employeeid =  o.employeeid
group by e.employeeid
having count(*) > 25


-- advance

-- 26. Find total sales amount per order.
--     Hint: Orders, Order Details, JOIN, GROUP BY


select o.orderid, sum(od.unitprice * od.quantity * (1 - od.discount)) as total_salse 
from order_details od join orders o
on o.orderid = od.orderid
group by o.orderid


-- 27. Find total sales amount per customer.
--     Hint: Customers, Orders, Order Details, JOIN, GROUP BY


select companyname ,sum(unitprice * quantity *(1-discount)) as total_sales
from customers c
    join orders o on c.customerid = o.customerid
    join order_details od on o.orderid = od.orderid
group by companyname


-- 28. Find top 10 products by total quantity sold.
--     Hint: Products, Order Details, JOIN, GROUP BY, ORDER BY


select  productname , sum(quantity) as total_quanity 
from products p  join order_details od
on  p.productid =  od.productid
group by p.productid 
order by total_quanity desc 
limit 10        --  to get top 10 high sold products list




-- 29. Find categories whose total sales are greater than 50000.
--     Hint: Categories, Products, Order Details, JOIN, GROUP BY, HAVING

 
select categoryname, sum(od.unitprice*od.quantity*(1-od.discount))as  totalsalse
from categories c  
             join products p on c.categoryid =  p.categoryid  
           join order_details od on p.productid =  od.productid

group by categoryname
having  sum(od.unitprice*od.quantity*(1-od.discount))> 50000


-- 30. Find employees whose total sales are greater than 100000.
--     Hint: Employees, Orders, Order Details, JOIN, GROUP BY, HAVING

select e.firstname ,sum(od.unitprice*od.quantity*(1-od.discount)) as total_sales
from employees e  join orders o
		on e.employeeid =  o.employeeid join
		order_details od
		on o.orderid =  od.orderid
group by e.employeeid
having sum(od.unitprice* od.quantity*(1-od.discount)) > 100000
		

-- 31. Find total sales per country based on customer country.
--     Hint: Customers, Orders, Order Details, JOIN, GROUP BY


select country, sum(od.unitprice*od.quantity*(1-od.discount)) as total_sales
from customers c  join orders o
		on c.customerid =  o.customerid join
		order_details od
		on o.orderid =  od.orderid
group by c.country


-- 32. Find suppliers whose products generated sales above 30000.
--     Hint: Suppliers, Products, Order Details, JOIN, GROUP BY, HAVING

select s.companyname ,sum(od.unitprice*od.quantity*(1-od.discount)) as total_sales
from suppliers s  join products p
		on s.supplierid =  p.supplierid join
		order_details od
		on p.productid =  od.productid
group by s.companyname
having sum(od.unitprice* od.quantity*(1-od.discount)) > 30000
		
-- 33. Find customers who placed more than 10 orders and sort by order count descending.
--     Hint: Customers, Orders, JOIN, GROUP BY, HAVING, ORDER BY

select companyname, count(*) as total_orders
from customers c join orders o
    on c.customerid =  o.customerid

group by c.customerid
having count(*)>10 
order by total_orders desc



-- 34. Find monthly order count for each year and month.
--     Hint: Orders, GROUP BY, ORDER BY

select  extract ( year from orderdate) as  year , 
extract (month from orderdate) as month, 
count(*) as order_count
			
from orders 
group by extract(year from orderdate), extract(month from orderdate)
order by extract(year from orderdate), extract(month from orderdate)


-- 35. Find monthly sales amount ordered by year and month.
--     Hint: Orders, Order Details, JOIN, GROUP BY, ORDER BY

select  extract ( year from orderdate) as  year , 
extract (month from orderdate) as month, 
sum(od.unitprice*od.quantity*(1-od.discount)) as monthly_sales
			
from orders o join order_details od
on o.orderid =  od.orderid

group by extract(year from orderdate), extract(month from orderdate)
order by extract(year from orderdate), extract(month from orderdate)
