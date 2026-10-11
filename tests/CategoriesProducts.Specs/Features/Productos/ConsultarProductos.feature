# language: es
Característica: Consultar productos
  Como usuario de la API
  Quiero ver la lista de productos y el detalle de cada uno
  Para saber qué productos tengo y a qué categoría pertenecen

  Antecedentes:
    Dado que existen las categorías:
      | nombre  | descripción       |
      | Bebidas | Refrescos y jugos |
      | Botanas | Papas y galletas  |

  Escenario: Ver la lista de productos con el nombre de su categoría
    Dado que existen los productos:
      | nombre    | descripción      | categoría |
      | Sprite    | Refresco de lima | Bebidas   |
      | Doritos   |                  | Botanas   |
      | Coca-Cola | Refresco de cola | Bebidas   |
    Cuando consulto la lista de productos
    Entonces la lista contiene 3 productos de un total de 3
    Y la lista muestra los productos en este orden:
      | nombre    | descripción      | categoría |
      | Coca-Cola | Refresco de cola | Bebidas   |
      | Doritos   |                  | Botanas   |
      | Sprite    | Refresco de lima | Bebidas   |

  Escenario: Ver la lista cuando no hay productos
    Cuando consulto la lista de productos
    Entonces la lista contiene 0 productos de un total de 0

  Escenario: La lista muestra 20 productos por página por defecto
    Dado que existen 25 productos en la categoría "Bebidas"
    Cuando consulto la lista de productos
    Entonces la lista contiene 20 productos de un total de 25
    Y la lista corresponde a la página 1 con 20 productos por página

  Esquema del escenario: Ver la lista de productos por páginas
    Dado que existen 25 productos en la categoría "Bebidas"
    Cuando consulto la página <página> de la lista de productos con <tamaño> productos por página
    Entonces la lista contiene <cantidad> productos de un total de 25
    Y la lista corresponde a la página <página> con <tamaño> productos por página

    Ejemplos:
      | página | tamaño | cantidad |
      | 1      | 10     | 10       |
      | 3      | 10     | 5        |
      | 4      | 10     | 0        |

  Esquema del escenario: No se puede consultar una página inválida
    Cuando consulto la página <página> de la lista de productos con <tamaño> productos por página
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "<mensaje>"

    Ejemplos:
      | página | tamaño | mensaje                                         |
      | 0      | 10     | La página debe ser mayor o igual a 1.           |
      | 1      | 0      | El tamaño de página debe estar entre 1 y 100.   |
      | 1      | 101    | El tamaño de página debe estar entre 1 y 100.   |

  Escenario: Ver el detalle de un producto con el id y el nombre de su categoría
    Dado que existe un producto con nombre "Coca-Cola" y descripción "Refresco de cola" en la categoría "Bebidas"
    Cuando consulto el detalle del producto "Coca-Cola"
    Entonces el producto se encuentra
    Y el producto consultado tiene el nombre "Coca-Cola"
    Y el producto consultado tiene la descripción "Refresco de cola"
    Y el producto consultado muestra el id y el nombre de la categoría "Bebidas"

  Escenario: El detalle del producto muestra el nombre actual de su categoría
    Dado que existe un producto con nombre "Coca-Cola" en la categoría "Bebidas"
    Y que cambio el nombre de la categoría "Bebidas" a "Refrescos"
    Cuando consulto el detalle del producto "Coca-Cola"
    Entonces el producto consultado muestra el id y el nombre de la categoría "Refrescos"

  Escenario: No se puede ver el detalle de un producto que no existe
    Cuando consulto el detalle de un producto que no existe
    Entonces el producto no se encuentra
    Y el mensaje de error es "No se encontró el producto."
