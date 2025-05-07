use pubs
go 

select * from publishers
select * from titleauthor
select * from titles
select * from authors
select * from sales
-- publishers who have not published any title 
select * from publishers where pub_id not in (select distinct pub_id from titles)
-- publishers who have published the titles
select * from publishers join titles on publishers.pub_id = titles.pub_id 
-- all the publishers who habe published as well as not published
select * from titles right outer join publishers on publishers.pub_id = titles.pub_id 
-- select auther_id for all books , print author_id and book name
select * from authors
select au_id, title from titleauthor join titles on titles.title_id = titleauthor.title_id

select concat(au_fname,' ',au_lname) Author_Name ,title Book_Name from authors a 
join titleauthor ta on a.au_id = ta.au_id 
join titles t on ta.title_id = ta.title_id

-- print the publisher's name, book name and the order date of the books
select pub_name, title, ord_date from publishers p 
join titles t on p.pub_id = t.pub_id
join sales s on t.title_id = s.title_id
order by 3 desc

-- print the publisher name and the first book sale date for all the publishers
select pub_name, min(ord_date) as first_order_date from publishers p 
join titles t on p.pub_id = t.pub_id
join sales s on t.title_id = s.title_id
group by pub_name
order by min(ord_date) -- sorts according to the min(ord_date)

-- print the bookname and the store address of the sale

select * from stores
select * from sales
select * from titles

select title Book_Name,stor_address Store_Address from titles t 
join sales s on t.title_id = s.title_id 
join stores st on s.stor_id = st.stor_id
order by 1

-- adhoc query 
-- store procedures -> pre-complied -> store and reuse -> direct execution

create procedure proc_sample
as 
begin
	print 'Hello World'
end
go
proc_sample

---------------------------------------------------------------------------------------------------

create table Products
(id int identity(1,1) constraint pk_productId primary key,
name nvarchar(100) not null,
details nvarchar(max))
go
create procedure proc_InsertProduct (@pname nvarchar(100), @pdetail nvarchar(max))
as
begin
	insert into Products(name, detail) values (@pname, @pdetail)
end
go
proc_InsertProduct '','{"brand":"Dell","spec":{"ram":"16GB","cpu":"i5"}}'
go
select * from Products

create proc proc_UpdateProductSpec(@pid int,@newvalue varchar(20))
as
begin
   update products set detail = JSON_MODIFY(detail, '$.spec.ram',@newvalue) where id = @pid
end

go 

proc_UpdateProductSpec 1, '24GB'

select * from Products

select * from Products where
try_cast(json_value(detail,'$.spec.cpu') as nvarchar(20)) = 'i5'

--------------------------------------------------------------------------------------------------------

  create table Posts
  (id int primary key,
  title nvarchar(100),
  user_id int,
  body nvarchar(max))
Go

  select * from Posts

  create proc proc_BulInsertPosts(@jsondata nvarchar(max))
  as
  begin
		insert into Posts(user_id,id,title,body)
	  select userId,id,title,body from openjson(@jsondata)
	  with (userId int,id int, title varchar(100), body varchar(max))
  end

  delete from Posts

  proc_BulInsertPosts '
[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]'

  ----------------------------------------------------------------------------------------------------------------------------------

  -- create a procedure that brings post by taking the user_id as the parameter

create procedure proc_GetPost 
@user_id int
as
begin
	select * from Posts where id = @user_id
end

proc_GetPost 1

 