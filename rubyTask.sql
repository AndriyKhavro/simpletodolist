--tables
CREATE TABLE [dbo].[projects](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_projects] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[tasks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[status] [nvarchar](50) NULL,
	[project_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


-- 1. get all statuses, not repeating, alphabetically ordered
SELECT DISTINCT T.[status]
	FROM tasks AS T
	ORDER BY T.[status]
	
--2. get the count of all tasks in each project, order by tasks count descending
SELECT P.name, COUNT(T.id) AS [Count]
	FROM tasks AS T
	RIGHT JOIN projects AS P
	ON T.project_id = P.id
	GROUP BY P.id, P.name
	ORDER BY [Count] DESC
	
--3. get the count of all tasks in each project, order by projects names
SELECT P.name AS Name, COUNT(T.id) AS [Count]
	FROM tasks AS T
	RIGHT JOIN projects AS P
	ON T.project_id = P.id
	GROUP BY P.id, P.name
	ORDER BY Name
	
--4. get the tasks for all projects having the name beginning with “N” letter
SELECT T.name, P.name AS project_name
	FROM tasks AS T
	INNER JOIN projects AS P
	ON T.project_id = P.id
	WHERE T.name LIKE 'N%'
	
--5. get the list of all projects containing the ‘a’ letter in the middle of the name, and show the
--tasks count near each project. Mention that there can exist projects without tasks and 
--tasks with project_id=NULL

SELECT P.name, COUNT(T.id)
	FROM projects AS P
	LEFT JOIN tasks AS T
	ON P.id = T.project_id
	WHERE P.name LIKE '%a%'
	GROUP BY P.id, P.name
	
--6. get the list of tasks with duplicate names. Order alphabetically
SELECT T.name
	FROM tasks AS T
	GROUP BY T.name
	HAVING COUNT(T.id) > 1
	ORDER BY T.name
	
--7. get the list of tasks having several exact matches of both name and status, from the
--project ‘Garage’. Order by matches count

SELECT T.name
	FROM tasks AS T
	INNER JOIN projects AS P
	ON T.project_id = P.id
	WHERE P.name = 'Garage'
	GROUP BY T.name, T.[status]
	HAVING COUNT(T.id) > 1
	ORDER BY COUNT(T.id)
	
--8.get the list of project names having more than 10 tasks in status ‘completed’. Order by project_id
SELECT P.name
	FROM projects AS P
	INNER JOIN tasks AS T
	ON P.id = T.project_id
	WHERE T.[status] = 'completed'
	GROUP BY P.id, P.name
	HAVING COUNT(T.id) > 10
	ORDER BY P.id