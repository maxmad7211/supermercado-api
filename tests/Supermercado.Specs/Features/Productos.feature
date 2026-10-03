# language: es
Característica: Gestión de productos
  Como usuario del catálogo
  Quiero crear, editar, consultar y eliminar productos
  Para mantener mi inventario organizado por categoría

  Antecedentes:
    Dado que existe la categoría "Bebidas"
    Y que existe la categoría "Snacks"

  # --- Crear ---

  Escenario: Crear un producto con nombre, descripción y categoría
    Cuando creo un producto con nombre "Agua", descripción "Agua natural 1L" y categoría "Bebidas"
    Entonces la respuesta debe tener el código 201
    Y el producto debe tener nombre "Agua" y descripción "Agua natural 1L"
    Y el producto debe pertenecer a la categoría "Bebidas"

  Escenario: Crear un producto sin descripción
    Cuando creo un producto con nombre "Agua" sin descripción en la categoría "Bebidas"
    Entonces la respuesta debe tener el código 201
    Y el producto debe tener nombre "Agua" sin descripción
    Y el producto debe pertenecer a la categoría "Bebidas"

  Escenario: No se puede crear un producto sin nombre
    Cuando creo un producto sin nombre en la categoría "Bebidas"
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "El nombre es requerido"

  Escenario: No se puede crear un producto sin categoría
    Cuando creo un producto con nombre "Agua" sin categoría
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "La categoria es obligatoria"

  Escenario: No se puede crear un producto en una categoría que no existe
    Cuando creo un producto con nombre "Agua" en una categoría que no existe
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "La categoria especificada no existe"

  # --- Editar ---

  Escenario: Editar un producto cambiando su categoría
    Dado que existe el producto "Papas" en la categoría "Bebidas"
    Cuando edito el producto "Papas" con nombre "Papas fritas", descripción "Sabor natural" y categoría "Snacks"
    Entonces la respuesta debe tener el código 200
    Y el producto debe tener nombre "Papas fritas" y descripción "Sabor natural"
    Y el producto debe pertenecer a la categoría "Snacks"

  Escenario: No se puede editar un producto dejándolo sin nombre
    Dado que existe el producto "Agua" en la categoría "Bebidas"
    Cuando edito el producto "Agua" sin nombre
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "El nombre es requerido"

  Escenario: No se puede editar un producto dejándolo sin categoría
    Dado que existe el producto "Agua" en la categoría "Bebidas"
    Cuando edito el producto "Agua" sin categoría
    Entonces la respuesta debe tener el código 400
    Y la respuesta debe contener el mensaje "La categoria es obligatoria"

  Escenario: No se puede editar un producto que no existe
    Cuando edito un producto que no existe
    Entonces la respuesta debe tener el código 404

  # --- Consultar ---

  Escenario: Ver un producto muestra el id y el nombre de su categoría
    Dado que existe el producto "Agua" en la categoría "Bebidas"
    Cuando consulto el producto "Agua"
    Entonces la respuesta debe tener el código 200
    Y el producto debe tener nombre "Agua" sin descripción
    Y el producto debe pertenecer a la categoría "Bebidas"

  Escenario: Ver el listado de productos con el nombre de su categoría
    Dado que existe el producto "Agua" en la categoría "Bebidas"
    Y que existe el producto "Papas" en la categoría "Snacks"
    Cuando consulto el listado de productos
    Entonces la respuesta debe tener el código 200
    Y el listado de productos debe ser:
      | Nombre | Categoría |
      | Agua   | Bebidas   |
      | Papas  | Snacks    |

  Escenario: Ver un producto que no existe
    Cuando consulto un producto que no existe
    Entonces la respuesta debe tener el código 404

  # --- Eliminar ---

  Escenario: Eliminar un producto
    Dado que existe el producto "Agua" en la categoría "Bebidas"
    Cuando elimino el producto "Agua"
    Entonces la respuesta debe tener el código 204
    Y el producto "Agua" ya no debe existir

  Escenario: No se puede eliminar un producto que no existe
    Cuando elimino un producto que no existe
    Entonces la respuesta debe tener el código 404
