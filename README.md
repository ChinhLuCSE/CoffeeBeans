# RoastCraft Labs — Take-Home Technical Challenge

## Important Information

- We value your time and recommend that you do not spend more than **4 hours** on this exercise.
- Don’t worry if you can’t complete everything in time—include your thoughts on what you’d do with more time.
- We want to understand your overall working and thought process: please submit a 1-pager that outlines your solution and key decisions, alongside the source code.
- The frontend is written in **NextJS** and the backend is written in **.NET 10**
Within this language constraint, you may use any third-party libraries you want (your choice of relational database, ORM, web framework, table rendering library, etc.).

## Problem Background

Imagine you work at **RoastCraft Labs**. RoastCraft Labs (RCL) collaborates with a large community of specialty roasters who use the RCL web platform to browse and analyze **coffee bean** metadata.

## Deliverables

- Source code for a web application that implements the 2 tasks outlined below.
- Instructions on how to run your submission locally.

## Task 1 — Bean Database and Table

Create a web application backed by a **relational database** that stores coffee-bean records and displays them in a page with a data table.

### Data

Use the provided JSON dataset (see “Coffee Beans Dataset” section below).

Each bean record contains:

- `bean_id` (string) — unique identifier (e.g., `BEAN000123456789`)
- `tasting_profile` (string) — compact note string (e.g., `cocoa+cherry/bergamot|honey`)
- `bag_weight_g` (float) — bag weight in grams
- `roast_index` (float) — a numeric index (can be negative to positive) indicating roast characteristics
- `num_farms` (integer) — count of farms contributing to the lot
- `num_acidity_notes` (integer) — number of distinct acidity notes detected
- `num_sweetness_notes` (integer) — number of distinct sweetness notes detected
- `x` and `y` (float) — 2D embedding coordinates used for plotting/similarity layouts

### The application should:

- Seed a relational database with the provided data.
- Render a page of results in a data table.
- Support **pagination**.
- Allow **sorting and filtering on multiple columns**.

## Task 2 — Dynamic Columns

Roasters want to add new, on-demand attributes to the bean table. Each new column can be **integer**, **float**, or **string**, but a given column must not mix types.

### Implement a feature so users can create a new column and see it in the table.

The application should:

- Allow users to specify the **name** of the new column and the **data type**.
- Automatically initialize the new column with **random values**.
- **Persist** this data in the database (modeling approach is up to you).
- Show the new column in the data table.

---

## Coffee Beans Dataset (500 rows)

A 500-row JSON file can be found here:

**[coffee_beans_500.json](https://raw.githubusercontent.com/techprep-gh/coffee-bean-library/main/coffee_beans_500.json)**
