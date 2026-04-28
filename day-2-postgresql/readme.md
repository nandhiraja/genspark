# PostgreSQL SQL Learning Notes

## Topics Learned

* `WHERE`
* `ORDER BY`
* `GROUP BY`
* `HAVING`
* `JOIN`
* Aggregate Functions
* Sales Queries
* Monthly Reports

---

# 1. WHERE → Filter Rows

Used to select specific rows.

```sql id="38o6gd"
SELECT *
FROM customers
WHERE country = 'USA';
```

### Common Operators

```text id="9wq6it"
=   >   <   >=   <=   !=
AND   OR
```

---

# 2. ORDER BY → Sort Data

Used to arrange output.

```sql id="u4uyy4"
SELECT *
FROM products
ORDER BY unitprice DESC;
```

| Keyword | Meaning    |
| ------- | ---------- |
| `ASC`   | Low → High |
| `DESC`  | High → Low |

---

# 3. Aggregate Functions

Used to calculate values from rows.

| Function  | Purpose    |
| --------- | ---------- |
| `COUNT()` | Count rows |
| `SUM()`   | Total      |
| `AVG()`   | Average    |
| `MAX()`   | Highest    |
| `MIN()`   | Lowest     |

Example:

```sql id="npr7om"
SELECT AVG(freight)
FROM orders;
```

---

# 4. GROUP BY → Create Groups

Groups similar values together.

```sql id="9f27sp"
SELECT country, COUNT(*) AS customer_count
FROM customers
GROUP BY country;
```

### Rule

When using `GROUP BY`:

* Non-aggregate columns must be grouped.

---

# 5. HAVING → Filter Groups

`WHERE` filters rows.
`HAVING` filters groups.

```sql id="9dq11e"
SELECT country, COUNT(*)
FROM customers
GROUP BY country
HAVING COUNT(*) > 5;
```

---

# 6. JOIN → Combine Tables

Used to connect related tables.

```sql id="ch4hbd"
SELECT orderid, companyname
FROM orders o
JOIN customers c
ON o.customerid = c.customerid;
```

### Real Usage

* Orders + Customers
* Products + Categories
* Orders + Employees

---

# 7. JOIN + GROUP BY

```sql id="4w56lu"
SELECT companyname, COUNT(*) AS total_orders
FROM customers c
JOIN orders o
ON c.customerid = o.customerid
GROUP BY companyname;
```

---

# 8. Sales Formula

```sql id="3vw0jx"
unitprice * quantity * (1 - discount)
```

Used to calculate total sales.

Example:

```sql id="trc6e6"
SELECT SUM(unitprice * quantity * (1-discount))
FROM order_details;
```

---

# 9. Top Records

```sql id="vylc8g"
SELECT productname, SUM(quantity) AS total_quantity
FROM products p
JOIN order_details od
ON p.productid = od.productid
GROUP BY productname
ORDER BY total_quantity DESC
LIMIT 10;
```

Used for:

* Top products
* Top customers
* Highest sales

---

# 10. Date Functions

Used `EXTRACT()` for monthly reports.

```sql id="n5w5fu"
SELECT
EXTRACT(YEAR FROM orderdate) AS year,
EXTRACT(MONTH FROM orderdate) AS month
FROM orders;
```

---

# 11. SQL Execution Order

```text id="xgm4ls"
FROM
→ JOIN
→ WHERE
→ GROUP BY
→ HAVING
→ SELECT
→ ORDER BY
→ LIMIT
```

---

# 12. Common Mistakes

### Wrong

```sql id="scf3r9"
WHERE COUNT(*) > 5
```

### Correct

```sql id="j0m5wg"
HAVING COUNT(*) > 5
```

---



# Quick Cheatsheet

## WHERE

```sql id="w0o84m"
WHERE condition
```

## ORDER BY

```sql id="m2vbvv"
ORDER BY column DESC
```

## GROUP BY

```sql id="4s92qz"
GROUP BY column
```

## HAVING

```sql id="mpjlwm"
HAVING COUNT(*) > 5
```

## JOIN

```sql id="0fvmmy"
JOIN table2
ON table1.id = table2.id
```

---

