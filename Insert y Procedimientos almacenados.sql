USE TastyNest
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
INSERT INTO Roles (RolName) VALUES (N'Administradores');
GO
INSERT INTO Roles (RolName) VALUES (N'Clientes');
GO

SELECT * FROM dbo.Roles 
GO

---- Inserciones en la tabla Users
-- Conflicto  con la insercion de usuario comentado por ello

--INSERT INTO Users (UserName, Email, PasswordHash, ProfileImage, RoleId)
--VALUES ('AdminUser', 'admin@example.com', 'hashedpassword123', 'admin_profile.jpg', 1);

--INSERT INTO Users (UserName, Email, PasswordHash, ProfileImage, RoleId)
--VALUES ('ClientUser', 'client@example.com', 'hashedpassword456', 'client_profile.jpg', 2);
--GO

-- Uso estos por mientras, no es recomendado tener el PlaintextPassword
INSERT INTO Users (IdentificationNumber, Name, Email, Password, Active, RoleId, UseTempPassword, Validity, FailedAttempts, LockedUntil, ValidatedEmail, VerificationToken)
VALUES 
(N'123456789', N'Admin User', N'admin@example.com', N'hashedpassword1', 1, 1, 0, GETDATE() + 365, 0, NULL, 1, ''),
(N'987654321', N'Client User', N'client@example.com', N'hashedpassword2', 1, 2, 0, GETDATE() + 365, 0, NULL, 1, '');
GO


-- Inserción en la tabla Categories

INSERT INTO Categories (Name) VALUES ('30-Minute Meals');
GO
INSERT INTO Categories (Name) VALUES ('5 Ingredients or Less');
GO
INSERT INTO Categories (Name) VALUES ('One-Pot Dishes');
GO
INSERT INTO Categories (Name) VALUES ('Casseroles & Bakes');
GO
INSERT INTO Categories (Name) VALUES ('Soups & Stews');
GO
INSERT INTO Categories (Name) VALUES ('Family Favorites');
GO
INSERT INTO Categories (Name) VALUES ('Italian');
GO
INSERT INTO Categories (Name) VALUES ('Asian');
GO
INSERT INTO Categories (Name) VALUES ('Mexican');
GO
INSERT INTO Categories (Name) VALUES ('Cakes & Cupcakes');
GO
INSERT INTO Categories (Name) VALUES ('Cookies & Bars');
GO
INSERT INTO Categories (Name) VALUES ('Pies & Desserts');
GO
INSERT INTO Categories (Name) VALUES ('Low-Carb');
GO
INSERT INTO Categories (Name) VALUES ('Vegan & Vegetarian');
GO
INSERT INTO Categories (Name) VALUES ('Gluten-Free');
GO
INSERT INTO Categories (Name) VALUES ('Roasts & Grills');
GO
INSERT INTO Categories (Name) VALUES ('Brunch Favorites');
GO
INSERT INTO Categories (Name) VALUES ('Party Meals');
GO

-- Inserciones en la tabla Recipes
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 1, 'Quick Spaghetti Carbonara', 'quick_spaghetti_carbonara.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 2, 'Simple Pancakes', 'simple_pancakes.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 3, 'One-Pot Chicken Alfredo', 'one_pot_chicken_alfredo.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 4, 'Cheesy Potato Bake', 'cheesy_potato_bake.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 5, 'Hearty Chicken Soup', 'hearty_chicken_soup.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 6, 'Grandma''s Meatloaf', 'grandmas_meatloaf.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 7, 'Classic Lasagna', 'classic_lasagna.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 8, 'Vegetable Stir-Fry', 'vegetable_stir_fry.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 9, 'Tacos al Pastor', 'tacos_al_pastor.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 10, 'Chocolate Lava Cake', 'chocolate_lava_cake.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 11, 'Chewy Chocolate Chip Cookies', 'chocolate_chip_cookies.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 12, 'Classic Apple Pie', 'classic_apple_pie.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 13, 'Zucchini Noodles with Pesto', 'zucchini_noodles_pesto.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 14, 'Vegan Buddha Bowl', 'vegan_buddha_bowl.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 15, 'Gluten-Free Brownies', 'gluten_free_brownies.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 16, 'Roast Beef Dinner', 'roast_beef_dinner.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 17, 'Brunch Quiche', 'brunch_quiche.jpg');
GO
INSERT INTO Recipes (UserId, CategoryId, Name, Image)
VALUES (1, 18, 'Party Sliders', 'party_sliders.jpg');
GO

-- Inserciones en la tabla Ingredients

INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (1, 'Spaghetti', '200g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (2, 'Flour', '2 cups');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (3, 'Chicken', '500g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (4, 'Potatoes', '3 large');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (5, 'Chicken breast', '300g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (6, 'Ground beef', '400g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (7, 'Lasagna noodles', '250g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (8, 'Mixed vegetables', '2 cups');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (9, 'Pork', '500g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (10, 'Chocolate', '200g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (11, 'Butter', '100g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (12, 'Apples', '4 large');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (13, 'Zucchini', '2 medium');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (14, 'Chickpeas', '1 cup');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (15, 'Cocoa powder', '50g');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (16, 'Beef roast', '1kg');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (17, 'Eggs', '4 large');
GO
INSERT INTO Ingredients (RecipeId, Name, Quantity)
VALUES (18, 'Mini burger buns', '6 pcs');
GO


-- Inserciones en la tabla RecipeSteps
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (1, 1, 'Boil spaghetti in salted water until al dente.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (1, 2, 'Mix with carbonara sauce and serve hot.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (2, 1, 'Combine flour, milk, and eggs in a bowl.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (2, 2, 'Cook on a hot griddle until golden brown.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (3, 1, 'Cook chicken and garlic in a pot.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (3, 2, 'Add pasta and Alfredo sauce, stir until coated.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (4, 1, 'Layer sliced potatoes and cheese in a baking dish.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (4, 2, 'Bake in the oven until golden and bubbly.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (5, 1, 'Saut onions, carrots, and celery in a pot.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (5, 2, 'Add chicken and broth, simmer until tender.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (6, 1, 'Mix ground beef with seasonings and breadcrumbs.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (6, 2, 'Bake in a loaf pan until fully cooked.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (7, 1, 'Layer lasagna noodles, sauce, and cheese.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (7, 2, 'Bake until bubbly and golden brown.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (8, 1, 'Stir-fry mixed vegetables in a wok with oil.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (8, 2, 'Add soy sauce and serve over rice.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (9, 1, 'Cook marinated pork on a grill or skillet.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (9, 2, 'Assemble tacos with pork, onions, and cilantro.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (10, 1, 'Prepare chocolate batter and pour into ramekins.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (10, 2, 'Bake until edges are firm but center is gooey.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (11, 1, 'Mix butter, sugar, and chocolate chips in a bowl.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (11, 2, 'Scoop dough onto baking sheet and bake.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (12, 1, 'Prepare crust and fill with apple mixture.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (12, 2, 'Bake until crust is golden and filling is bubbly.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (13, 1, 'Spiralize zucchini and prepare pesto sauce.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (13, 2, 'Mix zucchini noodles with pesto and serve.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (14, 1, 'Combine chickpeas, quinoa, and fresh veggies.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (14, 2, 'Drizzle with dressing and serve.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (15, 1, 'Mix cocoa powder, eggs, and almond flour.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (15, 2, 'Bake in a greased pan until set.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (16, 1, 'Season beef roast and sear on all sides.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (16, 2, 'Roast in the oven until desired doneness.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (17, 1, 'Prepare quiche crust and mix filling ingredients.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (17, 2, 'Bake until filling is set and golden.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (18, 1, 'Prepare mini burger patties and cook.');
GO
INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
VALUES (18, 2, 'Assemble sliders with buns and toppings.');
GO

-- DATO IMPORTANTE 
-- Para poder pasar este procedimiento ocupo que hagan esto vayan aqui 
--	Tools-->Options-->Query Execution-->SQL Server-->Advanced-->Enable Parameterization 
-- y Desactivan esa funcion la desmarcan basicamente y luego cierran y vuelven a abrir el sql server
-- al parecer hay algun tipo de conflicto con la parametrizacion de datos los BIT
-- que no permite hacerlo en un query comun y corriente esta es la unica soloucion a la que logre llegar
-- si encuentran otra perfecto.


CREATE PROCEDURE [dbo].[CreateUser]
    @IdentificationNumber NVARCHAR(20),
    @Name NVARCHAR(255),
    @Email NVARCHAR(80),
    @Password NVARCHAR(255),
	@VerificationToken NVARCHAR(100)
AS
BEGIN

    DECLARE @RolUsuario SMALLINT = 2;
    DECLARE @UsaClaveTemp BIT = 0;

    IF EXISTS (
        SELECT 1
        FROM dbo.Users
        WHERE IdentificationNumber = @IdentificationNumber
           OR Email = @Email
    )
    BEGIN
        PRINT 'Ya existe un usuario con el mismo número de identificación o correo electrónico.';
        RETURN; 
    END

    BEGIN
        INSERT INTO dbo.Users (IdentificationNumber, Name, Email, Password, Active, RoleId, UseTempPassword, Validity, FailedAttempts, LockedUntil, ValidatedEmail, VerificationToken)
        VALUES (@IdentificationNumber, @Name, @Email, @Password, 0, @RolUsuario, @UsaClaveTemp, GETDATE() + 365, 0, NULL, 0, @VerificationToken);

        PRINT 'Usuario creado exitosamente.';
    END

END;
GO


CREATE PROCEDURE [dbo].[Login]
    @Email NVARCHAR(80)
AS
BEGIN
    SELECT  U.Id,
            IdentificationNumber,
            Name,
            Email,
			Password,
            Active,
            RoleId,
            R.RolName,
            UseTempPassword,
            Validity,
			FailedAttempts,
			LockedUntil,
			ValidatedEmail
    FROM    dbo.Users U
    INNER JOIN dbo.Roles R ON U.RoleId = R.Id 
    WHERE   Email = @Email
        AND Active = 1
		OR Active = 0;
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

-- (Agregado RicardoA 01/12)
CREATE PROCEDURE GetFavoriteRecipes
    @UserId BIGINT
AS
BEGIN
    SELECT 
        r.Id AS RecipeId,
        r.Name AS RecipeName,
        c.Name AS CategoryName,
        (SELECT STRING_AGG(i.Name + ':' + i.Quantity, '; ')
         FROM Ingredients i
         WHERE i.RecipeId = r.Id) AS Ingredients,
        (SELECT STRING_AGG(CONVERT(VARCHAR, s.StepNumber) + ': ' + s.Description, '; ')
         FROM RecipeSteps s
         WHERE s.RecipeId = r.Id) AS Steps
    FROM Favorites f
    INNER JOIN Recipes r ON f.RecipeId = r.Id
    INNER JOIN Categories c ON r.CategoryId = c.Id
    WHERE f.UserId = @UserId
    ORDER BY r.Id;
END;
GO



-- (Agregado RicardoA 01/12)
CREATE PROCEDURE AgregarAFavoritos
    @UserId BIGINT,
    @RecipeId BIGINT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM Favorites 
        WHERE UserId = @UserId AND RecipeId = @RecipeId
    )
    BEGIN
        INSERT INTO Favorites (UserId, RecipeId, CreatedFavorites)
        VALUES (@UserId, @RecipeId, GETDATE());
    END
END;
GO

ALTER PROCEDURE GetFavoriteRecipes
    @UserId BIGINT
AS
BEGIN
    SELECT 
        r.Id AS RecipeId,
        r.Name AS RecipeName,
        c.Name AS CategoryName,
        i.Name AS IngredientName,
        i.Quantity AS IngredientQuantity,
        s.StepNumber AS StepNumber,
        s.Description AS StepDescription
    FROM Favorites f
    INNER JOIN Recipes r ON f.RecipeId = r.Id
    INNER JOIN Categories c ON r.CategoryId = c.Id
    LEFT JOIN Ingredients i ON r.Id = i.RecipeId
    LEFT JOIN RecipeSteps s ON r.Id = s.RecipeId
    WHERE f.UserId = @UserId
    ORDER BY r.Id, s.StepNumber;
END;
GO 


-- (Agregado RicardoA 05/12)
CREATE PROCEDURE DeleteRecipe
    @RecipeId BIGINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Eliminar los favoritos relacionados con la receta
        DELETE FROM Favorites
        WHERE RecipeId = @RecipeId;

        -- Eliminar los pasos de la receta
        DELETE FROM RecipeSteps
        WHERE RecipeId = @RecipeId;

        -- Eliminar los ingredientes de la receta
        DELETE FROM Ingredients
        WHERE RecipeId = @RecipeId;

        -- Eliminar la receta
        DELETE FROM Recipes
        WHERE Id = @RecipeId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE [dbo].[DeleteFavoritesByUserId]
    @UserId BIGINT
AS
BEGIN
    DELETE FROM Favorites
    WHERE UserId = @UserId;
END;
GO

CREATE PROCEDURE [dbo].[DeleteFavoriteByUserIdAndRecipeId]
    @UserId BIGINT,
    @RecipeId BIGINT
AS
BEGIN
    DELETE FROM Favorites
    WHERE UserId = @UserId AND RecipeId = @RecipeId;
END;
GO


-- Procedimiento para insertar una receta
CREATE PROCEDURE InsertRecipe
    @Name NVARCHAR(255),
    @CategoryId BIGINT,
    @UserId BIGINT,
    @Ingredients NVARCHAR(MAX), 
    @RecipeSteps NVARCHAR(MAX)  
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @RecipeId BIGINT;
        INSERT INTO Recipes (Name, CategoryId, UserId, Image, CreatedRecipes)
        VALUES (@Name, @CategoryId, @UserId, '', GETDATE());

        SET @RecipeId = SCOPE_IDENTITY();

        IF (@Ingredients IS NOT NULL)
        BEGIN
            -- Se parsea el JSON de ingredientes
            DECLARE @IngredientTable TABLE (
                Name NVARCHAR(100),
                Quantity NVARCHAR(100)
            );

            INSERT INTO @IngredientTable (Name, Quantity)
            SELECT 
                JSON_VALUE(Value, '$.Name') AS Name,
                JSON_VALUE(Value, '$.Quantity') AS Quantity
            FROM OPENJSON(@Ingredients);

            -- Insertar los ingredientes en la tabla Ingredients
            INSERT INTO Ingredients (RecipeId, Name, Quantity)
            SELECT @RecipeId, Name, Quantity
            FROM @IngredientTable;
        END

        -- Insertar pasos en la tabla RecipeSteps
        IF (@RecipeSteps IS NOT NULL)
        BEGIN
            -- Se parsea el JSON de pasos de la receta
            DECLARE @StepTable TABLE (
                StepNumber INT,
                Description NVARCHAR(500)
            );

            INSERT INTO @StepTable (StepNumber, Description)
            SELECT 
                JSON_VALUE(Value, '$.StepNumber') AS StepNumber,
                JSON_VALUE(Value, '$.Description') AS Description
            FROM OPENJSON(@RecipeSteps);

            -- Insertar los pasos en la tabla RecipeSteps
            INSERT INTO RecipeSteps (RecipeId, StepNumber, Description)
            SELECT @RecipeId, StepNumber, Description
            FROM @StepTable;
        END

        COMMIT TRANSACTION;

        -- Devolver el ID de la receta insertada
        SELECT @RecipeId AS RecipeId;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Lanzar el error capturado
        THROW;
    END CATCH
END;
GO



-- PROFILE
CREATE PROCEDURE [dbo].[CheckUserID]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 
           [Id],
           [IdentificationNumber],
           [Name],
           [Email],
           [Password],
           [Active],
           [RoleId],
           [UseTempPassword],
           [Validity],
           [CreatedUser],
		   [FailedAttempts],
		   [LockedUntil],
		   [ValidatedEmail],
		   [VerificationToken]
    FROM [TastyNest].[dbo].[Users]
    WHERE [Id] = @Id;
END
GO



CREATE PROCEDURE [dbo].[UpdateProfile]
    @Id BIGINT,
    @Name VARCHAR(255),
    @Email VARCHAR(80)
AS
BEGIN
    -- Verificar que no exista otro usuario con el mismo email o ID
    IF NOT EXISTS(
        SELECT 1 
        FROM dbo.Users
        WHERE (Email = @Email OR Id = @Id)
          AND Id != @Id
    )
    BEGIN
        -- Actualizar solo los campos Name y Email
        UPDATE dbo.Users
        SET Name = @Name,
            Email = @Email
        WHERE Id = @Id;
    END
END;
GO


CREATE PROCEDURE [dbo].[ValidarUsuario]
    @Email NVARCHAR(255)
AS
BEGIN
	
	SELECT
			U.Id,
			IdentificationNumber,
			Name,
			Email,
			Active,
			RoleId,
			R.RolName
	  FROM	dbo.Users U
	  INNER JOIN dbo.Roles R ON U.RoleId = R.Id
	  WHERE	U.Email = @Email

END
GO

CREATE PROCEDURE [dbo].[ActualizarContrasenna]
    @Id BIGINT,
	@Password VARCHAR(255),
	@UseTempPassword BIT,
	@Validity DATETIME
AS
BEGIN
	UPDATE dbo.Users
	   SET Password = @Password,
		   UseTempPassword = @UseTempPassword,
		   Validity = @Validity,
		   FailedAttempts = 0,
		   LockedUntil = NULL
	 WHERE Id = @Id
	
END
GO


CREATE PROCEDURE IncrementFailedAttempts
    @UserId BIGINT
AS
BEGIN
    UPDATE Users
    SET FailedAttempts = FailedAttempts + 1
    WHERE Id = @UserId;
END;
GO

CREATE PROCEDURE LockUserAccount
    @UserId BIGINT
AS
BEGIN
    UPDATE Users
    SET LockedUntil = DATEADD(MINUTE, 15, GETDATE()),
        FailedAttempts = 5
    WHERE Id = @UserId;
END;
GO

CREATE PROCEDURE ResetFailedAttempts
    @UserId BIGINT
AS
BEGIN
    UPDATE Users
    SET FailedAttempts = 0,
        LockedUntil = NULL
    WHERE Id = @UserId;
END;
GO

CREATE PROCEDURE DeleteUnvalidatedAccounts
AS
BEGIN
    DELETE FROM Users
    WHERE ValidatedEmail = 0 AND CreatedUser < DATEADD(HOUR, -1, GETDATE());
END;
GO

CREATE PROCEDURE [dbo].[CheckIfTokenExistsForEmail]
    @Email NVARCHAR(80)
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM dbo.Users 
        WHERE Email = @Email AND VerificationToken IS NOT NULL AND VerificationToken <> ''
    )
    BEGIN
        PRINT 'Existe un token asociado a este correo';
        RETURN 1; 
    END
    ELSE
    BEGIN
        PRINT 'No existe un token asociado a este correo';
        RETURN -1; 
    END
END;
GO

CREATE PROCEDURE [dbo].[VerifyToken]
    @VerificationToken NVARCHAR(100)
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM dbo.Users 
        WHERE VerificationToken = @VerificationToken
    )
    BEGIN
		UPDATE dbo.Users
		SET ValidatedEmail = 1,
			Active = 1,
			VerificationToken = '' 
		WHERE VerificationToken = @VerificationToken;
		PRINT 'Ya puedes iniciar sesion se ha verificado el token';
        RETURN 1; 
    END
	ELSE
    BEGIN
	    PRINT 'No existe tal token de verificacion';
		RETURN -1; 
	END
     
END;
GO

CREATE PROCEDURE [dbo].[DeleteUserAccount]
    @UserId BIGINT
AS
BEGIN
    DELETE FROM Users WHERE Id = @UserId;
END;
GO

CREATE PROCEDURE [dbo].[GetFeaturedRecipes]
AS
BEGIN
    SELECT TOP 9 Id, Name, Image 
    FROM Recipes 
END;
GO

CREATE PROCEDURE [dbo].[SearchRecipes]
	@SearchTerm NVARCHAR(MAX) 
AS
BEGIN
    SELECT r.Id, r.Name, c.Name AS CategoryName 
    FROM Recipes r
    INNER JOIN Categories c ON r.CategoryId = c.Id
    WHERE r.Name LIKE '%' + @SearchTerm + '%' OR 
          EXISTS (SELECT 1 FROM Ingredients i WHERE i.RecipeId = r.Id AND i.Name LIKE '%' + @SearchTerm + '%')
END;
GO


CREATE PROCEDURE [dbo].[GetRecipeDetails] 
    @RecipeId BIGINT 
AS
BEGIN
    SELECT 
        r.Id, 
        r.Name,
        (SELECT STRING_AGG(IngredientInfo, '; ') 
         FROM (SELECT DISTINCT i.Name + ':' + i.Quantity AS IngredientInfo 
               FROM Ingredients i 
               WHERE i.RecipeId = r.Id) AS DistinctIngredients) AS Ingredients,
        (SELECT STRING_AGG(StepInfo, '; ') 
         FROM (SELECT DISTINCT CONVERT(VARCHAR, s.StepNumber) + ': ' + s.Description AS StepInfo 
               FROM RecipeSteps s 
               WHERE s.RecipeId = r.Id) AS DistinctSteps) AS Steps
    FROM Recipes r 
    WHERE r.Id = @RecipeId;
END;
GO


CREATE PROCEDURE [dbo].[GetAllCategories]
AS
BEGIN
    SELECT Id, Name FROM dbo.Categories;
END;
GO


CREATE PROCEDURE [dbo].[GetRecipesByCategory]
    @CategoryId bigint
AS
BEGIN
    SELECT r.*
    FROM dbo.Recipes r
    INNER JOIN dbo.Categories c ON r.CategoryId = c.Id
    WHERE c.Id = @CategoryId;
END;
GO


CREATE PROCEDURE [dbo].[GetSubcategoriesByCategory]
    @ParentCategoryId bigint
AS
BEGIN
    SELECT c.Id, c.Name 
    FROM dbo.Categories c
    WHERE c.Id IN (
        SELECT CategoryId FROM dbo.Recipes r WHERE r.CategoryId = @ParentCategoryId
    );
END;
GO




--exec [dbo].[GetSubcategoriesByCategory] @ParentCategoryId=1 ;

-- NO TOCAR ESTO PORFAVOR SI SE QUIERE COMENZAR DESPUES DEL comentario que dice FINAL
SET XACT_ABORT OFF;

DECLARE @jobExists INT;

EXEC msdb.dbo.sp_help_job @job_name = N'EliminarCuentasNoValidadas';
IF @@ROWCOUNT > 0
BEGIN
    EXEC msdb.dbo.sp_delete_job @job_name = 'EliminarCuentasNoValidadas';
END


EXEC msdb.dbo.sp_add_job @job_name = 'EliminarCuentasNoValidadas';

EXEC msdb.dbo.sp_add_jobstep @job_name = 'EliminarCuentasNoValidadas',
    @step_name = 'Eliminar Cuentas',
    @command = 'EXEC DeleteUnvalidatedAccounts',
    @database_name = 'TastyNest';

EXEC msdb.dbo.sp_add_jobschedule @job_name = 'EliminarCuentasNoValidadas',
    @name = 'CadaHora',
    @freq_type = 4, 
    @freq_interval = 1,
    @freq_subday_type = 8, 
    @freq_subday_interval = 1;

SET XACT_ABORT ON;
-- FINAL LO DE ARRIBA ENCERADO NO TOCARLO

