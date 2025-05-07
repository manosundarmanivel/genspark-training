--1.How can you retrieve all the information from the cd.facilities table?
select * from cd.facilities

--2.You want to print out a list of all of the facilities and their cost to members. How would you retrieve a list of only facility names and costs?
select name, membercost from cd.facilities

--3.How can you produce a list of facilities that charge a fee to members?
select * from cd.facilities where membercost >0 

--4.How can you produce a list of facilities that charge a fee to members, and that fee is less than 1/50th of the monthly maintenance cost? Return the facid, facility name, member cost, and monthly maintenance of the facilities in question.
select facid, name, membercost, monthlymaintenance 
from cd.facilities 
where membercost > 0 AND membercost < monthlymaintenance/50

--5.How can you produce a list of all facilities with the word 'Tennis' in their name?
select * from cd.facilities where name like'%Tennis%'

--6.How can you retrieve the details of facilities with ID 1 and 5? Try to do it without using the OR operator.
select * from cd.facilities where facid in (1,5) 
--input can be given as the list or column too..

--7.How can you produce a list of facilities, with each labelled as 'cheap' or 'expensive' depending on if their monthly maintenance cost is more than $100? Return the name and monthly maintenance of the facilities in question
select name,
case
when monthlymaintenance >100 then 'expensive'
else 'cheap'
end as cost
from cd.facilities

--8.How can you produce a list of members who joined after the start of September 2012? Return the memid, surname, firstname, and joindate of the members in question.
select memid, surname, firstname, joindate from cd.members
where cast(joindate as date) >= '2012-09-01'
--or
where joindate >= '2012-09-01'; 

--9.How can you produce an ordered list of the first 10 surnames in the members table? The list must not contain duplicates.
select distinct surname from cd.members
order by surname
limit 10

--10.You, for some reason, want a combined list of all surnames and all facility names. Yes, this is a contrived example :-). Produce that list!
select surname from cd.members 
union
select name from cd.facilities 
-- union, union all(duplicates)

--11.You'd like to get the signup date of your last member. How can you retrieve this information?
select max(joindate) as latest from cd.members

--12.You'd like to get the first and last name of the last member(s) who signed up - not just the date. How can you do that?
select firstname, surname, joindate 
from cd.members
where joindate = (select max(joindate) as latest from cd.members)
-- or order by joindate and limt to 1

--13.How can you produce a list of the start times for bookings by members named 'David Farrell'?
select starttime from cd.bookings b 
join cd.members m on m.memid = b.memid
where m.firstname ='David' and m.surname = 'Farrell'

--14.How can you produce a list of the start times for bookings for tennis courts, for the date '2012-09-21'? Return a list of start time and facility name pairings, ordered by the time.
select starttime start, name from cd.bookings b
join cd.facilities f on b.facid = f.facid 
where cast(starttime as date) = '2012-09-21'
and name like '%Tennis Court%'
order by starttime

--15.How can you output a list of all members who have recommended another member? Ensure that there are no duplicates in the list, and that results are ordered by (surname, firstname).
select distinct r.firstname, r.surname from cd.members m
inner join cd.members r on r.memid = m.recommendedby
order by surname, firstname

--16.How can you output a list of all members, including the individual who recommended them (if any)? Ensure that results are ordered by (surname, firstname).
select  m.firstname memfname, m.surname memsname, r.firstname recfname, r.surname recsname from cd.members m
left outer join cd.members r on r.memid = m.recommendedby
order by memsname,memfname





