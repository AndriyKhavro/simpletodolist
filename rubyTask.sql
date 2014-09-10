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


-- 1. get all statuses, not repeating, alph
SELECT DISTINCT T.[status]
	FROM tasks AS T
	ORDER BY T.[status]
	
--2. 
SELECT P.name, COUNT(T.id) AS [Count]
	FROM tasks AS T
	RIGHT JOIN projects AS P
	ON T.project_id = P.id
	GROUP BY P.id, P.name
	ORDER BY [Count] DESC
	
--3.
SELECT P.name AS Name, COUNT(T.id) AS [Count]
	FROM tasks AS T
	RIGHT JOIN projects AS P
	ON T.project_id = P.id
	GROUP BY P.id, P.name
	ORDER BY Name
	
--4.
SELECT T.name, P.name AS project_name
	FROM tasks AS T
	INNER JOIN projects AS P
	ON T.project_id = P.id
	WHERE T.name LIKE 'N%'
	
--5.
SELECT P.name, COUNT(T.id)
	FROM projects AS P
	LEFT JOIN tasks AS T
	ON P.id = T.project_id
	WHERE P.name LIKE '%a%'
	GROUP BY P.id, P.name
	
--6.
SELECT T.name
	FROM tasks AS T
	GROUP BY T.name
	HAVING COUNT(T.id) > 1
	ORDER BY T.name
	
--7.
SELECT T.name
	FROM tasks AS T
	INNER JOIN projects AS P
	ON T.project_id = P.id
	WHERE P.name = 'Garage'
	GROUP BY T.name, T.[status]
	HAVING COUNT(T.id) > 1
	ORDER BY COUNT(T.id)
	
--8.
SELECT P.name
	FROM projects AS P
	INNER JOIN tasks AS T
	ON P.id = T.project_id
	WHERE T.[status] = 'completed'
	GROUP BY P.id, P.name
	HAVING COUNT(T.id) > 10
	ORDER BY P.id