# 🧠 Price Notifier Solution

Este es un sistema distribuido desarrollado en .NET 8 que demuestra el uso de **Azure Service Bus (con Topics y Subscriptions)** para publicar y consumir actualizaciones de 
precios. Es ideal para proyectos donde múltiples servicios necesitan reaccionar a un mismo evento con responsabilidades distintas.

---

## 🧱 Estructura del proyecto
PriceNotifierSolution/

- Producer.Api/ # Publica mensajes de actualización de precio en un topic
- Consumer.Worker/ # Procesa las actualizaciones de precios recibidas
- Alertas/ # Escucha cambios de precios y genera alertas
- Facturas/ # Genera facturas a partir de los mensajes
- Shared/ # Contiene modelos compartidos y lógica común


---

## 🚀 Tecnologías utilizadas

- [.NET 8](https://dotnet.microsoft.com/)
- Azure Service Bus (Topics & Subscriptions)
- Azure Container Apps
- GitHub Actions
- Docker + GHCR
- Logging con `ILogger`

---

## 🔄 Flujo de funcionamiento

1. **Producer.Api** expone un endpoint para recibir precios actualizados.
2. El mensaje se publica en un **Azure Service Bus Topic** (`price-updates`).
3. Cada worker (`Consumer`, `Alertas`, `Facturas`) está suscripto con su propia **Subscription**:
   - `ConsumerSubscription`
   - `AlertasSubscription`
   - `FacturasSubscription`
4. Cada uno consume el mensaje y realiza su tarea:
   - El consumer lo procesa
   - Alertas puede disparar notificaciones
   - Facturas puede emitir comprobantes

---

## 🐳 Imágenes Docker (GHCR)

Las imágenes están publicadas en [GitHub Container Registry (GHCR)](https://ghcr.io):
ghcr.io/facunav/producer-api
ghcr.io/facunav/consumer-worker
ghcr.io/facunav/alertas
ghcr.io/facunav/facturas


---

## 🧪 Cómo probar localmente

1. Clonar el repo:
   ```bash
   git clone https://github.com/facunav/PriceNotifierSolution.git
   cd PriceNotifierSolution

2. Agregar tus secrets locales (appsettings.Development.json o secrets de .NET):
{
  "ServiceBus": {
    "ConnectionString": "<tu-connection-string>",
    "TopicName": "price-updates",
    "ConsumerSubscriptionName": "ConsumerSubscription",
    "FacturasSubscriptionName": "FacturasSubscription",
    "AlertasSubscriptionName": "AlertasSubscription"
  }
}

3. Levantar desde Visual Studio o terminal:
dotnet run --project Producer.Api

☁️ Despliegue en Azure Container Apps
Cada componente fue desplegado como un Container App independiente, con CI/CD desde GitHub Actions y deploy de imágenes desde GHCR.

- Producer y todos los workers se conectan al mismo Azure Service Bus Topic.

- Las revisiones pueden gestionarse desde Azure Portal si alguna falla.
