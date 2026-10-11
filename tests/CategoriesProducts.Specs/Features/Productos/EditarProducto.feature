# language: es
Característica: Editar productos
  Como usuario de la API
  Quiero editar los datos de un producto, incluida su categoría
  Para mantener mi catálogo actualizado

  Antecedentes:
    Dado que existen las categorías:
      | nombre  | descripción       |
      | Bebidas | Refrescos y jugos |
      | Botanas | Papas y galletas  |
    Y que existe un producto con nombre "Coca-Cola" y descripción "Refresco de cola" en la categoría "Bebidas"

  Escenario: Editar el nombre y la descripción de un producto
    Cuando edito el producto "Coca-Cola" con nombre "Coca-Cola Zero", descripción "Refresco sin azúcar" y categoría "Bebidas"
    Entonces el producto se actualiza correctamente
    Y al consultar el detalle del producto "Coca-Cola Zero" tiene la descripción "Refresco sin azúcar"

  Escenario: Cambiar un producto de categoría
    Cuando edito el producto "Coca-Cola" con nombre "Coca-Cola", descripción "Refresco de cola" y categoría "Botanas"
    Entonces el producto se actualiza correctamente
    Y al consultar el detalle del producto "Coca-Cola" muestra el id y el nombre de la categoría "Botanas"

  Escenario: Quitar la descripción de un producto
    Cuando edito el producto "Coca-Cola" con nombre "Coca-Cola" sin descripción en la categoría "Bebidas"
    Entonces el producto se actualiza correctamente
    Y al consultar el detalle del producto "Coca-Cola" no tiene descripción

  Escenario: Los espacios al inicio y al final del nombre se ignoran al editar
    Cuando edito el producto "Coca-Cola" con nombre "  Coca-Cola Zero  " sin descripción en la categoría "Bebidas"
    Entonces el producto se actualiza correctamente
    Y el producto actualizado tiene el nombre "Coca-Cola Zero"

  Escenario: No se puede editar un producto dejándolo sin nombre
    Cuando edito el producto "Coca-Cola" sin nombre en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede editar un producto con nombre vacío
    Cuando edito el producto "Coca-Cola" con nombre "" sin descripción en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede editar un producto con un nombre de solo espacios
    Cuando edito el producto "Coca-Cola" con nombre "   " sin descripción en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre es requerido."

  Escenario: No se puede editar un producto con un nombre demasiado largo
    Cuando edito el producto "Coca-Cola" con un nombre de 101 caracteres en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "El nombre no puede tener más de 100 caracteres."

  Escenario: No se puede editar un producto con una descripción demasiado larga
    Cuando edito el producto "Coca-Cola" con nombre "Coca-Cola" y una descripción de 501 caracteres en la categoría "Bebidas"
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La descripción no puede tener más de 500 caracteres."

  Escenario: No se puede editar un producto dejándolo sin categoría
    Cuando edito el producto "Coca-Cola" con nombre "Coca-Cola" sin categoría
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La categoría es requerida."

  Escenario: No se puede mover un producto a una categoría que no existe
    Cuando edito el producto "Coca-Cola" con nombre "Coca-Cola" en una categoría que no existe
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "La categoría indicada no existe."
    Y al consultar el detalle del producto "Coca-Cola" muestra el id y el nombre de la categoría "Bebidas"

  Escenario: No se puede editar un producto que no existe
    Cuando edito un producto que no existe con nombre "Coca-Cola" en la categoría "Bebidas"
    Entonces el producto no se encuentra
    Y el mensaje de error es "No se encontró el producto."
