namespace PoderJudicial.Data
{
    /// <summary>
    /// El driver Microsoft ACE OLEDB (usado para Access/Jet) no es seguro
    /// para accesos concurrentes desde varios hilos al mismo tiempo: si dos
    /// operaciones le pegan simultáneamente (por ejemplo, Dashboard cargando
    /// las tablas y HomePage cargando sus totales, justo después del login),
    /// el componente COM interno truena con SEHException
    /// ("External component has thrown an exception").
    ///
    /// Esta clase expone un semáforo global de un solo "turno" para que
    /// todo el código que llame a la BD en background pase por aquí y
    /// nunca se ejecuten dos operaciones OleDb al mismo tiempo.
    /// </summary>
    public static class AccesoBD
    {
        private static readonly SemaphoreSlim _semaforo = new(1, 1);

        public static async Task<T> EjecutarAsync<T>(Func<T> operacion)
        {
            await _semaforo.WaitAsync();
            try
            {
                return await Task.Run(operacion);
            }
            finally
            {
                _semaforo.Release();
            }
        }
    }
}
