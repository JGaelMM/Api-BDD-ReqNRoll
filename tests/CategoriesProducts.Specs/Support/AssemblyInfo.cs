// Todos los escenarios comparten la misma base de datos y la limpian al iniciar,
// así que no pueden correr en paralelo.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
