// Manejo del botón para Crear Receta
document.getElementById('createRecipeButton').addEventListener('click', function () {
    const form = document.getElementById('createRecipeForm');
    const name = document.getElementById('recipeName').value.trim();
    const categoryId = parseInt(document.getElementById('categoryId').value);
    const ingredientsInput = document.getElementById('ingredients').value.trim();
    const stepsInput = document.getElementById('steps').value.trim();

    form.classList.add('was-validated');
    if (!name || !categoryId || !ingredientsInput || !stepsInput) {
        alert("Todos los campos son obligatorios.");
        return;
    }

    try {
        // Validar y construir los ingredientes
        const ingredients = ingredientsInput.split(';').map(item => {
            const parts = item.split(',');
            if (parts.length < 2) {
                throw new Error("Cada ingrediente debe tener nombre y cantidad separados por una coma.");
            }
            return { Name: parts[0].trim(), Quantity: parts[1].trim() };
        });

        // Validar y construir los pasos
        const steps = stepsInput.split(';').map((desc, index) => {
            if (!desc.trim()) {
                throw new Error("Cada paso debe contener una descripción.");
            }
            return { StepNumber: index + 1, Description: desc.trim() };
        });

        // Enviar los datos al backend
        fetch('https://localhost:7044/api/Recetas/CrearReceta', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                Name: name,
                CategoryId: categoryId,
                Ingredients: ingredients,
                RecipeSteps: steps // Clave corregida
            }),
        })
            .then(response => {
                if (!response.ok) return response.text().then(text => { throw new Error(text); });
                return response.json();
            })
            .then(data => {
                alert(`Receta creada exitosamente: ${data.Name}`);
                agregarRecetaAlDOM(data);
                form.reset(); // Reiniciar el formulario
                document.querySelector('#createRecipeModal .btn-close').click(); // Cerrar el modal
            })
            .catch(err => alert(`Error al crear receta: ${err.message}`));
    } catch (err) {
        alert(`Error: ${err.message}`);
    }
});

// Función para cargar las recetas favoritas del usuario
function cargarRecetasFavoritas() {
    fetch('https://localhost:7044/api/Recetas/ObtenerRecetasFavoritas?userId=1')
        .then(response => {
            if (!response.ok) throw new Error("Error al obtener recetas favoritas");
            return response.json();
        })
        .then(data => {
            const recipeCards = document.getElementById('recipeCards');
            recipeCards.innerHTML = '';

            if (!data || !data.$values || !Array.isArray(data.$values)) {
                console.error("La respuesta no contiene datos válidos.");
                return;
            }

            const recipes = data.$values;

            // Crear las cards para las recetas favoritas
            recipes.forEach(recipe => {
                agregarRecetaAlDOM(recipe);
            });
        })
        .catch(err => console.error('Error al cargar recetas favoritas:', err));
}

// Función para agregar una receta al DOM
function agregarRecetaAlDOM(recipe) {
    const recipeCards = document.getElementById('recipeCards');
    const card = `
        <div class="col-md-4 mb-4">
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title fw-bold">${recipe.recipeName || recipe.Name || "Sin nombre"}</h5>
                    <p><strong>Categoría:</strong> ${recipe.categoryName || recipe.CategoryName || "Sin categoría"}</p>
                    <p><strong>Ingredientes:</strong></p>
                    <ul>
                        ${(recipe.Ingredients || recipe.ingredients || []).map(ing => `<li>${ing.Name || "Desconocido"}, ${ing.Quantity || ""}</li>`).join('')}
                    </ul>
                    <p><strong>Pasos:</strong></p>
                    <ul>
                        ${(recipe.RecipeSteps || recipe.steps || []).map(step => `<li>Paso ${step.StepNumber}: ${step.Description}</li>`).join('')}
                    </ul>
                    <div class="d-flex justify-content-between">
                        <button class="btn btn-warning text-white btn-sm">Editar</button>
                        <button class="btn btn-danger text-white btn-sm" data-recipe-id="${recipe.recipeId || recipe.Id}">Eliminar</button>
                    </div>
                </div>
            </div>
        </div>
    `;
    recipeCards.innerHTML += card;

    // Vincular el botón "Eliminar" a la función eliminarReceta
    document.querySelectorAll('.btn-danger').forEach(button => {
        button.addEventListener('click', () => {
            const recipeId = button.getAttribute('data-recipe-id');
            eliminarReceta(recipeId);
        });
    });
}

// Manejador para eliminar una receta
function eliminarReceta(recipeId) {
    if (!confirm("¿Estás seguro de que deseas eliminar esta receta?")) return;

    fetch(`https://localhost:7044/api/Recetas/EliminarReceta/${recipeId}`, {
        method: 'DELETE',
    })
        .then(response => {
            if (!response.ok) return response.text().then(text => { throw new Error(text); });
            return response.json();
        })
        .then(data => {
            alert(data.Message);

            // Eliminar la card correspondiente en la interfaz
            document.querySelector(`[data-recipe-id="${recipeId}"]`).closest('.col-md-4').remove();
        })
        .catch(err => alert(`Error al eliminar la receta: ${err.message}`));
}

// Cargar recetas favoritas al cargar la página
document.addEventListener("DOMContentLoaded", cargarRecetasFavoritas);
