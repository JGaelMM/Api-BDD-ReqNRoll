# language: es
Característica: Crear categorías
  Como usuario de la API
  Quiero crear categorías
  Para poder organizar mis productos

  Escenario: Crear una categoría con nombre y descripción
    Cuando creo una categoría con nombre "Bebidas" y descripción "Refrescos, jugos y agua"
    Entonces la categoría se crea correctamente
    Y la categoría creada tiene el nombre "Bebidas"
    Y la categoría creada tiene la descripción "Refrescos, jugos y agua"

  Escenario: Crear una categoría sin descripción
    Cuando creo una categoría con nombre "Lácteos" sin descripción
    Entonces la categoría se crea correctamente
    Y la categoría creada tiene el nombre "Lácteos"
    Y la categoría creada no tiene descripción

  Escenario: Los espacios al inicio y al final del nombre se ignoran
    Cuando creo una categoría con nombre "  Bebidas  " sin descripción
    Entonces la categoría se crea correctamente
    Y la categoría creada tiene el nombre "Bebidas"

  Escenario: No se puede crear una categoría sin nombre
    Cuando creo una categoría sin nombre
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede crear una categoría con nombre vacío
    Cuando creo una categoría con nombre "" sin descripción
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede crear una categoría con un nombre de solo espacios
    Cuando creo una categoría con nombre "   " sin descripción
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede crear una categoría con un nombre demasiado largo
    Cuando creo una categoría con un nombre de 101 caracteres
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre no puede tener más de 100 caracteres."

  Escenario: No se puede crear una categoría con una descripción demasiado larga
    Cuando creo una categoría con nombre "Bebidas" y una descripción de 501 caracteres
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La descripción no puede tener más de 500 caracteres."

  Esquema del escenario: No se puede repetir el nombre de una categoría sin importar mayúsculas y minúsculas
    Dado que existe una categoría con nombre "Bebidas"
    Cuando creo una categoría con nombre "<nombre>" sin descripción
    Entonces la solicitud es rechazada por conflicto
    Y el mensaje de error es "Ya existe una categoría con el nombre '<nombre>'."

    Ejemplos:
      | nombre  |
      | Bebidas |
      | bebidas |
      | BEBIDAS |
      | bEbIdAs |

  Escenario: No se puede repetir el nombre aunque tenga espacios al inicio o al final
    Dado que existe una categoría con nombre "Bebidas"
    Cuando creo una categoría con nombre "  bebidas  " sin descripción
    Entonces la solicitud es rechazada por conflicto
