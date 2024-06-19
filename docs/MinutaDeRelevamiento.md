# Minuta de Relevamiento  (En Proceso) 

Proyecto: Sistema de Venta de Indumentaria
Responsables: [Ignacio Bastinelli, Francesco D’Agostino, Aylen Guy, Danilo Mercado]
-----------------------------------------------------------------------------------

# Contexto 

El objetivo de nuestro proyecto es desarrollar un sistema de venta de indumentaria que permita 
gestionar el funcionamiento de la tienda a través de ocho entidades clave. Estas entidades incluyen: 
Usuario (clase abstracta), Super Admin (hereda de Usuario), Admin (hereda de Usuario), 
Cliente (hereda de Usuario), Venta, Linea, Producto. El sistema proporcionará una plataforma en línea 
donde los usuarios podrán comprar productos de indumentaria y gestionar sus compras de manera eficiente.

# Proceso Actual

En el sistema actual, la venta y gestión de productos de indumentaria se realiza de manera manual o a través de un sistema. 
Los administradores y super administradores deben gestionar productos, pedidos y pagos sin una plataforma centralizada que 
automatice y agilice estos procesos. Los clientes tienen una experiencia de compra limitada y no pueden gestionar sus perfiles 
o historial de compras de manera efectiva.

# Registro y Gestión de Usuarios

- Usuarios del Sistema:
  - Super Admin: Acceso total al sistema; gestiona usuarios y productos.
  - Admin: Gestiona productos, pedidos, pagos y genera reportes.
  - Cliente: Registra, inicia sesión, compra productos, gestiona perfil e historial de compras.
- Registro de Usuarios:
  - Los clientes se registran con datos personales.
  - Super admin crea administradores y super administradores.

# Gestión de Productos
   - Agregar Productos: Admins añaden productos con nombre, descripción, precio, stock y categoría.
   - Modificar y Eliminar Productos: Admins actualizan o eliminan productos del catálogo.

# Proceso de Compra
   - Carro de Compras: Clientes agregan productos al carro; se muestra lista con cantidad y precio total.
   - Pagos: Clientes seleccionan método de pago; el sistema procesa el pago y actualiza el estado del pedido.


# Gestión de Pedidos
  - Estados de los Pedidos:
  - Pendiente: Pedido creado, esperando procesamiento.
  - En Proceso: Pedido en preparación para envío.
  - Enviado: Pedido enviado al cliente.
  - Completado: Pedido entregado al cliente.
  - Cancelado: Pedido cancelado por cliente o administrador.
  - Notificaciones: Sistema notifica a clientes sobre el estado de sus pedidos vía correo electrónico.

# Estados de una Consulta
  - Esperando respuesta del administrador: Consulta hecha, esperando respuesta del administrador.
  - Esperando respuesta del cliente: Administrador ha respondido, esperando intervención del cliente.
  - Resuelta: Consulta considerada resuelta por cliente o administrador.
  - Cancelada: Consulta cancelada por inactividad o resolución imposible.

# Requerimientos Funcionales
  - Usuarios (Clase Abstracta): Propiedades y métodos comunes (Id, Nombre, Email, Contraseña, Login, Logout).
  - Super Admin: Gestión completa del sistema.
  - Admin: Gestión de productos, pedidos, pagos y reportes.
  - Cliente: Registro, inicio de sesión, gestión de perfil y pedidos.
  - Producto: Propiedades y métodos de gestión.
  - Carro: Propiedades y métodos de gestión del carro de compras.
  - Pagos: Propiedades y métodos de procesamiento de pagos.

# Requerimientos No Funcionales
  - Rendimiento: Soporte para hasta 1000 usuarios simultáneos.
  - Seguridad: Autenticación robusta, encriptación y protección contra ataques.
  - Usabilidad: Interfaz intuitiva y responsive.
  - Mantenibilidad: Código limpio y modular.

—--------------------------------------------------------------------------------------------------------------
# DIAGRAMA DE CLASES 
https://excalidraw.com/#room=0583fdf45f8db3d108da,6tQYtA1nFVZPWHU_UUuHag
