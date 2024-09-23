CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Role" VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS "Projects" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "UserId" INT NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "WorkItems" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "DueDate" TIMESTAMP NOT NULL,
    "Status" INT NOT NULL,
    "Priority" INT NOT NULL,
    "ProjectId" INT NOT NULL,
    FOREIGN KEY ("ProjectId") REFERENCES "Projects"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "WorkItemHistory" (
    "Id" SERIAL PRIMARY KEY,
    "WorkItemId" INT NOT NULL,
    "ChangedBy" VARCHAR(100) NOT NULL,
    "ChangedAt" TIMESTAMP NOT NULL,
    "ChangeDescription" TEXT,
    FOREIGN KEY ("WorkItemId") REFERENCES "WorkItems"("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "WorkItemComment" (
    "Id" SERIAL PRIMARY KEY,
    "WorkItemId" INT NOT NULL,
    "CommentText" TEXT NOT NULL,
    "CommentedBy" VARCHAR(100) NOT NULL,
    "CommentedAt" TIMESTAMP NOT NULL,
    FOREIGN KEY ("WorkItemId") REFERENCES "WorkItems"("Id") ON DELETE CASCADE
);

INSERT INTO "Users" ("Name", "Email", "Role") VALUES 
('John Doe', 'johndoe@example.com', 'Manager');

INSERT INTO "Projects" ("Name", "Description", "UserId") VALUES 
('Projeto Exemplo', 'Este é um projeto de exemplo', 1);

INSERT INTO "WorkItems" ("Title", "Description", "DueDate", "Status", "Priority", "ProjectId") VALUES 
('Tarefa 1', 'Esta é a primeira tarefa', NOW() + INTERVAL '7 days', 1, 3, 1),
('Tarefa 2', 'Esta é a segunda tarefa', NOW() + INTERVAL '14 days', 2, 2, 1);

INSERT INTO "WorkItemHistory" ("WorkItemId", "ChangedBy", "ChangedAt", "ChangeDescription") VALUES 
(1, 'John Doe', NOW(), 'Tarefa criada'),
(2, 'John Doe', NOW() - INTERVAL '1 day', 'Tarefa em andamento');

INSERT INTO "WorkItemComment" ("WorkItemId", "CommentText", "CommentedBy", "CommentedAt") VALUES 
(1, 'Este é um comentário inicial', 'John Doe', NOW()),
(2, 'Progresso da tarefa', 'John Doe', NOW() - INTERVAL '1 day');
