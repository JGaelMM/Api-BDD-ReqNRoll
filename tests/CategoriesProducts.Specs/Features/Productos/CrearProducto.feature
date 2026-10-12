# language: es
Característica: Crear productos
  Como usuario de la API
  Quiero registrar productos dentro de una categoría
  Para tener mi catálogo organizado

  Antecedentes:
    Dado que existe una categoría con nombre "Bebidas"

  Escenario: Crear un producto con nombre, descripción y categoría
    Cuando creo un producto con nombre "Coca-Cola", descripción "Refresco de cola" y categoría "Bebidas"
    Entonces el producto se crea correctamente
    Y el producto creado tiene el nombre "Coca-Cola"
    Y el producto creado tiene la descripción "Refresco de cola"
    Y el producto creado pertenece a la categoría "Bebidas"

  Escenario: Crear un producto sin descripción
    Cuando creo un producto con nombre "Coca-Cola" sin descripción en la categoría "Bebidas"
    Entonces el producto se crea correctamente
    Y el producto creado no tiene descripción
    Y el producto creado pertenece a la categoría "Bebidas"

  Escenario: Los espacios al inicio y al final del nombre se ignoran
    Cuando creo un producto con nombre "  Coca-Cola  " sin descripción en la categoría "Bebidas"
    Entonces el producto se crea correctamente
    Y el producto creado tiene el nombre "Coca-Cola"

  Esquema del escenario: No se puede repetir el nombre de un producto sin importar mayúsculas y minúsculas
    Dado que existe un producto con nombre "Coca-Cola" en la categoría "Bebidas"
    Cuando creo un producto con nombre "<nombre>" sin descripción en la categoría "Bebidas"
    Entonces la solicitud es rechazada por conflicto
    Y el mensaje de error es "Ya existe un producto con el nombre '<nombre>'."

    Ejemplos:
      | nombre    |
      | Coca-Cola |
      | coca-cola |
      | COCA-COLA |
      | cOcA-cOlA |

  Escenario: No se puede repetir el nombre de un producto aunque tenga espacios al inicio o al final
    Dado que existe un producto con nombre "Coca-Cola" en la categoría "Bebidas"
    Cuando creo un producto con nombre "  coca-cola  " sin descripción en la categoría "Bebidas"
    Entonces la solicitud es rechazada por conflicto

  Escenario: No se puede repetir el nombre de un producto aunque esté en otra categoría
    Dado que existe una categoría con nombre "Refrescos"
    Y que existe un producto con nombre "Coca-Cola" en la categoría "Bebidas"
    Cuando creo un producto con nombre "coca-cola" sin descripción en la categoría "Refrescos"
    Entonces la solicitud es rechazada por conflicto
    Y el mensaje de error es "Ya existe un producto con el nombre 'coca-cola'."

  Escenario: No se puede crear un producto sin nombre
    Cuando creo un producto sin nombre en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede crear un producto con nombre vacío
    Cuando creo un producto con nombre "" sin descripción en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede crear un producto con un nombre de solo espacios
    Cuando creo un producto con nombre "   " sin descripción en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede crear un producto con un nombre demasiado largo
    Cuando creo un producto con un nombre de 101 caracteres en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre no puede tener más de 100 caracteres."

  Escenario: No se puede crear un producto con una descripción demasiado larga
    Cuando creo un producto con nombre "Coca-Cola" y una descripción de 501 caracteres en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La descripción no puede tener más de 500 caracteres."

  Escenario: No se puede crear un producto sin categoría
    Cuando creo un producto con nombre "Coca-Cola" sin categoría
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La categoría es requerida."

  Escenario: No se puede crear un producto en una categoría que no existe
    Cuando creo un producto con nombre "Coca-Cola" en una categoría que no existe
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La categoría indicada no existe."
