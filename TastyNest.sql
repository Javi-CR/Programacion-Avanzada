
CREATE DATABASE TastyNest;
GO

USE TastyNest;
GO

-- Roles
CREATE TABLE Roles (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Usuarios
CREATE TABLE Users (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    ProfileImage NVARCHAR(255),
    RoleId BIGINT NOT NULL,
    CreatedUser DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);
GO

-- Categorķas
CREATE TABLE Categories (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Recetas
CREATE TABLE Recipes (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    UserId BIGINT NOT NULL, 
    CategoryId BIGINT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(2000), 
    Image NVARCHAR(255),
    CreatedRecipes DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) 
);
GO

-- Ingredientes
CREATE TABLE Ingredients (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    RecipeId BIGINT NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Quantity NVARCHAR(50)
    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id) ON DELETE CASCADE
);
GO

-- Preparacion
CREATE TABLE RecipeSteps (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    RecipeId BIGINT NOT NULL,
    StepNumber INT NOT NULL,
    Description NVARCHAR(2000) NOT NULL,
    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id) ON DELETE CASCADE
);
GO

-- Favoritos
CREATE TABLE Favorites (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    UserId BIGINT NOT NULL,
    RecipeId BIGINT NOT NULL,
    CreatedFavorites DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Favorites_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Favorites_Recipes FOREIGN KEY (RecipeId) REFERENCES Recipes(Id) ON DELETE NO ACTION
);
GO