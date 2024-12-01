
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

-- Categorï¿½as
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

-- Cambios RicardoA

-- Eliminar la columna Descripcion por ser innecesaria.
IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Recipes' AND COLUMN_NAME = 'Description'
)
BEGIN
    ALTER TABLE Recipes DROP COLUMN Description;
END;
GO

-- INSERTs para las tablas:

-- Inserciones en la tabla Roles
INSERT INTO Roles (Name) VALUES ('admin');
INSERT INTO Roles (Name) VALUES ('cliente');
GO

-- Inserciones en la tabla Users
INSERT INTO Users (UserName, Email, PasswordHash, ProfileImage, RoleId)
VALUES ('AdminUser', 'admin@example.com', 'hashedpassword123', 'admin_profile.jpg', 1);

INSERT INTO Users (UserName, Email, PasswordHash, ProfileImage, RoleId)
VALUES ('ClientUser', 'client@example.com', 'hashedpassword456', 'client_profile.jpg', 2);
GO


-- Inserción en la tabla Categories
INSERT INTO Categories (Name) VALUES ('30-Minute Meals');
INSERT INTO Categories (Name) VALUES ('5 Ingredients or Less');
INSERT INTO Categories (Name) VALUES ('One-Pot Dishes');
INSERT INTO Categories (Name) VALUES ('Casseroles & Bakes');
INSERT INTO Categories (Name) VALUES ('Soups & Stews');
INSERT INTO Categories (Name) VALUES ('Family Favorites');
INSERT INTO Categories (Name) VALUES ('Italian');
INSERT INTO Categories (Name) VALUES ('Asian');
INSERT INTO Categories (Name) VALUES ('Mexican');
INSERT INTO Categories (Name) VALUES ('Cakes & Cupcakes');
INSERT INTO Categories (Name) VALUES ('Cookies & Bars');
INSERT INTO Categories (Name) VALUES ('Pies & Desserts');
INSERT INTO Categories (Name) VALUES ('Low-Carb');
INSERT INTO Categories (Name) VALUES ('Vegan & Vegetarian');
INSERT INTO Categories (Name) VALUES ('Gluten-Free');
INSERT INTO Categories (Name) VALUES ('Roasts & Grills');
INSERT INTO Categories (Name) VALUES ('Brunch Favorites');
INSERT INTO Categories (Name) VALUES ('Party Meals');
GO

-- Inserciones en la tabla Recipes
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 1, 'Quick Spaghetti Carbonara', 'quick_spaghetti_carbonara.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 2, 'Simple Pancakes', 'simple_pancakes.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 3, 'One-Pot Chicken Alfredo', 'one_pot_chicken_alfredo.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 4, 'Cheesy Potato Bake', 'cheesy_potato_bake.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 5, 'Hearty Chicken Soup', 'hearty_chicken_soup.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 6, 'Grandma’s Meatloaf', 'grandmas_meatloaf.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 7, 'Classic Lasagna', 'classic_lasagna.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 8, 'Vegetable Stir-Fry', 'vegetable_stir_fry.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 9, 'Tacos al Pastor', 'tacos_al_pastor.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 10, 'Chocolate Lava Cake', 'chocolate_lava_cake.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 11, 'Chewy Chocolate Chip Cookies', 'chocolate_chip_cookies.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 12, 'Classic Apple Pie', 'classic_apple_pie.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 13, 'Zucchini Noodles with Pesto', 'zucchini_noodles_pesto.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 14, 'Vegan Buddha Bowl', 'vegan_buddha_bowl.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 15, 'Gluten-Free Brownies', 'gluten_free_brownies.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 16, 'Roast Beef Dinner', 'roast_beef_dinner.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 17, 'Brunch Quiche', 'brunch_quiche.jpg');
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 18, 'Party Sliders', 'party_sliders.jpg');
GO

-- Inserciones en la tabla Ingredients
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (1, 'Spaghetti', '200g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (2, 'Flour', '2 cups');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (3, 'Chicken', '500g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (4, 'Potatoes', '3 large');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (5, 'Chicken breast', '300g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (6, 'Ground beef', '400g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (7, 'Lasagna noodles', '250g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (8, 'Mixed vegetables', '2 cups');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (9, 'Pork', '500g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (10, 'Chocolate', '200g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (11, 'Butter', '100g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (12, 'Apples', '4 large');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (13, 'Zucchini', '2 medium');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (14, 'Chickpeas', '1 cup');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (15, 'Cocoa powder', '50g');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (16, 'Beef roast', '1kg');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (17, 'Eggs', '4 large');
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (18, 'Mini burger buns', '6 pcs');
GO


-- Inserciones en la tabla RecipeSteps
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (1, 1, 'Boil spaghetti in salted water until al dente.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (1, 2, 'Mix with carbonara sauce and serve hot.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (2, 1, 'Combine flour, milk, and eggs in a bowl.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (2, 2, 'Cook on a hot griddle until golden brown.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (3, 1, 'Cook chicken and garlic in a pot.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (3, 2, 'Add pasta and Alfredo sauce, stir until coated.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (4, 1, 'Layer sliced potatoes and cheese in a baking dish.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (4, 2, 'Bake in the oven until golden and bubbly.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (5, 1, 'Sauté onions, carrots, and celery in a pot.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (5, 2, 'Add chicken and broth, simmer until tender.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (6, 1, 'Mix ground beef with seasonings and breadcrumbs.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (6, 2, 'Bake in a loaf pan until fully cooked.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (7, 1, 'Layer lasagna noodles, sauce, and cheese.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (7, 2, 'Bake until bubbly and golden brown.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (8, 1, 'Stir-fry mixed vegetables in a wok with oil.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (8, 2, 'Add soy sauce and serve over rice.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (9, 1, 'Cook marinated pork on a grill or skillet.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (9, 2, 'Assemble tacos with pork, onions, and cilantro.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (10, 1, 'Prepare chocolate batter and pour into ramekins.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (10, 2, 'Bake until edges are firm but center is gooey.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (11, 1, 'Mix butter, sugar, and chocolate chips in a bowl.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (11, 2, 'Scoop dough onto baking sheet and bake.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (12, 1, 'Prepare crust and fill with apple mixture.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (12, 2, 'Bake until crust is golden and filling is bubbly.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (13, 1, 'Spiralize zucchini and prepare pesto sauce.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (13, 2, 'Mix zucchini noodles with pesto and serve.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (14, 1, 'Combine chickpeas, quinoa, and fresh veggies.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (14, 2, 'Drizzle with dressing and serve.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (15, 1, 'Mix cocoa powder, eggs, and almond flour.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (15, 2, 'Bake in a greased pan until set.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (16, 1, 'Season beef roast and sear on all sides.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (16, 2, 'Roast in the oven until desired doneness.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (17, 1, 'Prepare quiche crust and mix filling ingredients.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (17, 2, 'Bake until filling is set and golden.');

INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (18, 1, 'Prepare mini burger patties and cook.');
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (18, 2, 'Assemble sliders with buns and toppings.');
GO



-- CREACION DE PROCEDIMIENTOS ALMACENADOS

CREATE PROCEDURE InsertRecipe
    @Name NVARCHAR(100),
    @CategoryId BIGINT,
    @UserId BIGINT
AS
BEGIN
    INSERT INTO Recipes (Name, CategoryId, UserId)
    VALUES (@Name, @CategoryId, @UserId);

    SELECT SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE InsertIngredient
    @RecipeId BIGINT,
    @Name NVARCHAR(50),
    @Quantity NVARCHAR(50)
AS
BEGIN
    INSERT INTO Ingredients (RecipeId, Name, Quantity)
    VALUES (@RecipeId, @Name, @Quantity);
END;
GO

CREATE PROCEDURE InsertRecipeStep
    @RecipeId BIGINT,
    @StepNumber INT,
    @Description NVARCHAR(2000)
AS
BEGIN
    INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
    VALUES (@RecipeId, @StepNumber, @Description);
END;
GO

-- (Agregado RicardoA 01/12)
CREATE PROCEDURE InsertFavorite
    @UserId BIGINT,
    @RecipeId BIGINT
AS
BEGIN
    INSERT INTO Favorites (UserId, RecipeId, CreatedFavorites)
    VALUES (@UserId, @RecipeId, GETDATE());
END;
GO


