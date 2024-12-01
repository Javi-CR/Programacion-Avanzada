document.getElementById('createRecipeButton').addEventListener('click', function () {
    const form = document.getElementById('createRecipeForm');
    const name = document.getElementById('recipeName').value.trim();
    const categoryId = document.getElementById('categoryId').value;
    const ingredientsInput = document.getElementById('ingredients').value.trim();
    const stepsInput = document.getElementById('steps').value.trim();

    // Validar campos
    form.classList.add('was-validated');
    if (!name || !categoryId || !ingredientsInput || !stepsInput) {
        return;
    }

    // Validar formato de ingredientes
    const ingredients = ingredientsInput.split(';').map(item => {
        const [name, quantity] = item.split(',');
        if (!name || !quantity) {
            alert("Formato de ingredientes incorrecto. Ejemplo: 'Harina, 2 tazas; Azúcar, 1 taza'");
            return;
        }
        return { Name: name.trim(), Quantity: quantity.trim() };
    });

    // Validar formato de pasos
    const steps = stepsInput.split(';').map((desc, index) => {
        if (!desc.trim()) {
            alert("Formato de pasos incorrecto. Ejemplo: 'Mezclar los ingredientes; Hornear'");
            return;
        }
        return { StepNumber: index + 1, Description: desc.trim() };
    });

    if (!ingredients || !steps) {
        return;
    }

    // Enviar datos al servidor
    fetch('https://localhost:7044/api/Recetas/CrearReceta', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Name: name, CategoryId: categoryId, Ingredients: ingredients, Steps: steps }),
    })
        .then(response => {
            if (!response.ok) throw new Error("Error en la creación de la receta");
            return response.json();
        })
        .then(data => {
            alert(data.Message);
            location.reload();
        })
        .catch(err => alert('Error: ' + err.message));
});

// Funcion parar cargar las recetas favoritas del usuario
document.addEventListener("DOMContentLoaded", () => {
    fetch('/Recetas/ObtenerRecetasFavoritas')
        .then(response => response.json())
        .then(data => {
            const recipeCards = document.getElementById('recipeCards');
            recipeCards.innerHTML = '';

            data.forEach(recipe => {
                const card = `
                    <div class="col-md-4 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title fw-bold">${recipe.Name}</h5>
                                <p><strong>Ingredientes:</strong></p>
                                <ul>${recipe.Ingredients.map(ing => `<li>${ing.Name}, ${ing.Quantity}</li>`).join('')}</ul>
                                <p><strong>Pasos:</strong></p>
                                <ul>${recipe.Steps.map(step => `<li>${step.Description}</li>`).join('')}</ul>
                                <div class="d-flex justify-content-between">
                                    <button class="btn btn-warning text-white btn-sm">Editar</button>
                                    <button class="btn btn-danger text-white btn-sm">Eliminar</button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                recipeCards.innerHTML += card;
            });
        })
        .catch(err => console.error('Error al cargar recetas favoritas:', err));
});

