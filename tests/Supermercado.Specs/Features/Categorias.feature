# language: es
Característica: Gestión de categorías
  Como usuario del catálogo
  Quiero crear, editar, consultar y eliminar categorías
  Para organizar mis productos

  # --- Crear ---

  Escenario: Crear una categoría con nombre y descripción
    Cuando creo una categoría con nombre "Bebidas" y descripción "Refrescos y jugos"
    Entonces la respuesta debe tener el código 201
    Y la categoría debe tener nombre "Bebidas" y descripción "Refrescos y jugos"

  Escenario: Crear una categoría sin descripción
    Cuando creo una categoría con nombre "Lácteos" sin descripción
    Entonces la respuesta debe tener el código 201
    Y la categoría debe tener nombre "Lácteos" sin descripción

  Escenario: No se puede crear una categoría sin nombre
    Cuando creo una categoría sin nombre
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "El nombre es requerido"

  Escenario: No se puede crear una categoría con nombre en blanco
    Cuando creo una categoría con nombre "   " sin descripción
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "El nombre es requerido"

  Esquema del escenario: No se puede repetir el nombre de una categoría sin importar mayúsculas
    Dado que existe la categoría "Bebidas"
    Cuando creo una categoría con nombre "<nombre>" sin descripción
    Entonces la respuesta debe tener el código 409
    Y la respuesta debe contener el mensaje "Ya existe una categoria con el nombre '<nombre>'"

    Ejemplos:
      | nombre  |
      | Bebidas |
      | BEBIDAS |
      | bebidas |
      | bEbIdAs |

  # --- Editar ---

  Escenario: Editar una categoría
    Dado que existe la categoría "Bebidas"
    Cuando edito la categoría "Bebidas" con nombre "Bebidas frías" y descripción "Solo bebidas frías"
    Entonces la respuesta debe tener el código 200
    Y la categoría debe tener nombre "Bebidas frías" y descripción "Solo bebidas frías"

  Escenario: Editar una categoría cambiando solo las mayúsculas de su propio nombre
    Dado que existe la categoría "bebidas"
    Cuando edito la categoría "bebidas" con nombre "Bebidas" y descripción "Ahora con mayúscula"
    Entonces la respuesta debe tener el código 200
    Y la categoría debe tener nombre "Bebidas" y descripción "Ahora con mayúscula"

  Escenario: No se puede editar una categoría dejándola sin nombre
    Dado que existe la categoría "Bebidas"
    Cuando edito la categoría "Bebidas" sin nombre
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "El nombre es requerido"

  Esquema del escenario: No se puede editar una categoría con el nombre de otra sin importar mayúsculas
    Dado que existe la categoría "Bebidas"
    Y que existe la categoría "Snacks"
    Cuando edito la categoría "Snacks" con nombre "<nombre>" y descripción "Duplicada"
    Entonces la respuesta debe tener el código 409
    Y la respuesta debe contener el mensaje "Ya existe una categoria con el nombre '<nombre>'"

    Ejemplos:
      | nombre  |
      | Bebidas |
      | BEBIDAS |
      | bebidas |

  Escenario: No se puede editar una categoría que no existe
    Cuando edito una categoría que no existe
    Entonces la respuesta debe tener el código 404

  # --- Consultar ---

  Escenario: Ver el listado de categorías (INDEX)
    Dado que existe la categoría "Bebidas" con descripción "Refrescos"
    Y que existe la categoría "Snacks"
    Cuando consulto el listado de categorías
    Entonces la respuesta debe tener el código 200
    Y el listado de categorías debe ser:
      | Nombre  | Descripción |
      | Bebidas | Refrescos   |
      | Snacks  |             |

  Escenario: Ver el detalle de una categoría (SHOW)
    Dado que existe la categoría "Bebidas" con descripción "Refrescos"
    Y que existe el producto "Agua" en la categoría "Bebidas"
    Cuando consulto la categoría "Bebidas"
    Entonces la respuesta debe tener el código 200
    Y la categoría debe tener nombre "Bebidas" y descripción "Refrescos"
    Y la categoría debe mostrar los productos:
      | Nombre |
      | Agua   |

  Escenario: Ver una categoría que no existe
    Cuando consulto una categoría que no existe
    Entonces la respuesta debe tener el código 404

  # --- Eliminar ---

  Escenario: Eliminar una categoría sin productos
    Dado que existe la categoría "Bebidas"
    Cuando elimino la categoría "Bebidas"
    Entonces la respuesta debe tener el código 204
    Y la categoría "Bebidas" ya no debe existir

  Escenario: No se puede eliminar una categoría con productos asignados
    Dado que existe la categoría "Bebidas"
    Y que existe el producto "Agua" en la categoría "Bebidas"
    Cuando elimino la categoría "Bebidas"
    Entonces la respuesta debe tener el código 409
    Y la respuesta debe contener el mensaje "Esta categoria no se puede eliminar porque tiene productos asignados"

  Escenario: Se puede eliminar una categoría después de eliminar todos sus productos
    Dado que existe la categoría "Bebidas"
    Y que existe el producto "Agua" en la categoría "Bebidas"
    Y que existe el producto "Refresco" en la categoría "Bebidas"
    Cuando elimino el producto "Agua"
    Y elimino el producto "Refresco"
    Y elimino la categoría "Bebidas"
    Entonces la respuesta debe tener el código 204
    Y la categoría "Bebidas" ya no debe existir

  Escenario: No se puede eliminar una categoría que no existe
    Cuando elimino una categoría que no existe
    Entonces la respuesta debe tener el código 404
