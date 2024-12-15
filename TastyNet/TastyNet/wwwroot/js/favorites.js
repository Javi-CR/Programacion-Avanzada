// Manejo del botón para Crear Receta
document.getElementById('createRecipeButton').addEventListener('click', function () {
    const form = document.getElementById('createRecipeForm');
    const name = document.getElementById('recipeName').value.trim();
    const categoryId = parseInt(document.getElementById('categoryId').value);
    const userId = 1; // Hardcoded UserId (cámbialo a dinámico si corresponde)

    form.classList.add('was-validated');
    if (!name || !categoryId) {
        alert("Todos los campos son obligatorios.");
        return;
    }

    // Recolectar ingredientes y pasos dinámicamente
    const ingredients = collectIngredients();
    const steps = collectSteps();

    if (ingredients.length === 0 || steps.length === 0) {
        alert("Debe incluir al menos un ingrediente y un paso.");
        return;
    }

    // JSON a enviar
    const requestData = {
        name,
        categoryId,
        userId,
        ingredients,
        recipeSteps: steps
    };

    console.log("Enviando datos al backend:", requestData);

    // Llamada al API para crear la receta
    fetch('https://localhost:7044/api/Recetas/CrearReceta', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestData),
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }
            return response.json();
        })
        .then(data => {
            alert(data.Message);
            location.reload(); // Recargar la página tras crear la receta
        })
        .catch(err => {
            console.error(`Error al crear receta: ${err.message}`);
            alert(`Error al crear receta: ${err.message}`);
        });
});

// Función para cargar las recetas favoritas del usuario
function cargarRecetasFavoritas() {
    fetch('https://localhost:7044/api/Recetas/ObtenerRecetasFavoritas?userId=1')
        .then(response => {
            if (!response.ok) {
                throw new Error("Error al obtener recetas favoritas");
            }
            return response.json();
        })
        .then(data => {
            const recipeCards = document.getElementById('recipeCards');
            recipeCards.innerHTML = '';

            const recipes = data.$values || [];

            recipes.forEach(recipe => {
                const card = `
                    <div class="col-md-4 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title fw-bold">${recipe.recipeName || "Sin nombre"}</h5>
                                <p><strong>Categoría:</strong> ${recipe.categoryName || "Sin categoría"}</p>
                                <p><strong>Ingredientes:</strong></p>
                                <ul>
                                    ${recipe.ingredients
                        ?.map(i => `<li>${i.name}, ${i.quantity}</li>`)
                        .join("") || "<li>Sin ingredientes</li>"
                    }
                                </ul>
                                <p><strong>Pasos:</strong></p>
                                <ul>
                                    ${recipe.steps
                        ?.map(s => `<li>Paso ${s.stepNumber}: ${s.description}</li>`)
                        .join("") || "<li>Sin pasos</li>"
                    }
                                </ul>
                                <div class="d-flex justify-content-between">
                                    <button class="btn btn-warning text-white btn-sm">Editar</button>
                                    <button class="btn btn-danger text-white btn-sm" data-recipe-id="${recipe.recipeId}">Eliminar</button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                recipeCards.innerHTML += card;
            });

            // Vincular el botón "Eliminar" a la función eliminarReceta
            document.querySelectorAll('.btn-danger').forEach(button => {
                button.addEventListener('click', () => {
                    const recipeId = button.getAttribute('data-recipe-id');
                    eliminarReceta(recipeId);
                });
            });
        })
        .catch(err => {
            console.error('Error al cargar recetas favoritas:', err);
            alert('Error al cargar recetas favoritas.');
        });
}

// Manejador para eliminar una receta
function eliminarReceta(recipeId) {
    if (!confirm("¿Estás seguro de que deseas eliminar esta receta?")) return;

    fetch(`https://localhost:7044/api/Recetas/EliminarReceta/${recipeId}`, {
        method: 'DELETE',
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }
            return response.json();
        })
        .then(data => {
            alert(data.Message);

            // Eliminar la card correspondiente en la interfaz
            const card = document.querySelector(`[data-recipe-id="${recipeId}"]`).closest('.col-md-4');
            if (card) card.remove();
        })
        .catch(err => {
            console.error(`Error al eliminar la receta: ${err.message}`);
            alert(`Error al eliminar la receta: ${err.message}`);
        });
}

// Función para recolectar ingredientes desde inputs dinámicos
function collectIngredients() {
    const ingredients = [];
    document.querySelectorAll(".ingredient-row").forEach(row => {
        const name = row.querySelector(".ingredient-name").value.trim();
        const quantity = row.querySelector(".ingredient-quantity").value.trim();
        if (name && quantity) ingredients.push({ name, quantity });
    });
    return ingredients;
}

// Función para recolectar pasos desde inputs dinámicos
function collectSteps() {
    const steps = [];
    document.querySelectorAll(".step-row").forEach((row, index) => {
        const description = row.querySelector(".step-description").value.trim();
        if (description) steps.push({ stepNumber: index + 1, description });
    });
    return steps;
}

// Cargar recetas favoritas al cargar la página
document.addEventListener("DOMContentLoaded", cargarRecetasFavoritas);
