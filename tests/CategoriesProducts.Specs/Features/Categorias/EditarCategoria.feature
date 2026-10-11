# language: es
Característica: Editar categorías
  Como usuario de la API
  Quiero editar el nombre y la descripción de una categoría
  Para mantener mis categorías actualizadas

  Antecedentes:
    Dado que existe una categoría con nombre "Bebidas" y descripción "Refrescos y jugos"

  Escenario: Editar el nombre y la descripción de una categoría
    Cuando edito la categoría "Bebidas" con nombre "Bebidas frías" y descripción "Refrescos, jugos y agua"
    Entonces la categoría se actualiza correctamente
    Y al consultar el detalle de la categoría "Bebidas frías" tiene la descripción "Refrescos, jugos y agua"

  Escenario: Quitar la descripción de una categoría
    Cuando edito la categoría "Bebidas" con nombre "Bebidas" sin descripción
    Entonces la categoría se actualiza correctamente
    Y al consultar el detalle de la categoría "Bebidas" no tiene descripción

  Escenario: Una categoría puede conservar su nombre cambiando mayúsculas y minúsculas
    Cuando edito la categoría "Bebidas" con nombre "BEBIDAS" sin descripción
    Entonces la categoría se actualiza correctamente
    Y la categoría actualizada tiene el nombre "BEBIDAS"

  Escenario: Los espacios al inicio y al final del nombre se ignoran al editar
    Cuando edito la categoría "Bebidas" con nombre "  Bebidas frías  " sin descripción
    Entonces la categoría se actualiza correctamente
    Y la categoría actualizada tiene el nombre "Bebidas frías"

  Escenario: No se puede editar una categoría dejándola sin nombre
    Cuando edito la categoría "Bebidas" sin nombre
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede editar una categoría con nombre vacío
    Cuando edito la categoría "Bebidas" con nombre "" sin descripción
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede editar una categoría con un nombre de solo espacios
    Cuando edito la categoría "Bebidas" con nombre "   " sin descripción
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede editar una categoría con un nombre demasiado largo
    Cuando edito la categoría "Bebidas" con un nombre de 101 caracteres
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre no puede tener más de 100 caracteres."

  Escenario: No se puede editar una categoría con una descripción demasiado larga
    Cuando edito la categoría "Bebidas" con nombre "Bebidas" y una descripción de 501 caracteres
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La descripción no puede tener más de 500 caracteres."

  Esquema del escenario: No se puede usar el nombre de otra categoría sin importar mayúsculas y minúsculas
    Dado que existe una categoría con nombre "Lácteos"
    Cuando edito la categoría "Bebidas" con nombre "<nombre>" sin descripción
    Entonces la solicitud es rechazada por conflicto
    Y el mensaje de error es "Ya existe una categoría con el nombre '<nombre>'."

    Ejemplos:
      | nombre  |
      | Lácteos |
      | lácteos |
      | LÁCTEOS |

  Escenario: Un nombre rechazado no modifica la categoría
    Dado que existe una categoría con nombre "Lácteos"
    Cuando edito la categoría "Bebidas" con nombre "Lácteos" sin descripción
    Entonces la solicitud es rechazada por conflicto
    Y al consultar el detalle de la categoría "Bebidas" tiene la descripción "Refrescos y jugos"

  Escenario: No se puede editar una categoría que no existe
    Cuando edito una categoría que no existe con nombre "Bebidas" sin descripción
    Entonces la categoría no se encuentra
    Y el mensaje de error es "No se encontró la categoría."
