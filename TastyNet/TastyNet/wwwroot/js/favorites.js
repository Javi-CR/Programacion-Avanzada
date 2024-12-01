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
