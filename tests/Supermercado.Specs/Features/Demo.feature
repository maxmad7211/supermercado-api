# language: es
Característica: Reinicio de datos de demostración
  Como evaluador de la API
  Quiero dejar la base vacía y con los ids desde 1
  Para que los ejemplos precargados en Scalar coincidan con los datos

  Escenario: Reiniciar borra todo y los ids vuelven a empezar en 1
    Dado que existe la categoría "Bebidas"
    Y que existe la categoría "Snacks"
    Y que existe el producto "Agua" en la categoría "Bebidas"
    Cuando reinicio los datos de demostración
    Entonces la respuesta debe tener el código 204
    Y el listado de categorías debe estar vacío
    Y el listado de productos debe estar vacío
    Cuando creo una categoría con nombre "Lácteos" sin descripción
    Entonces la categoría creada debe tener el id 1
