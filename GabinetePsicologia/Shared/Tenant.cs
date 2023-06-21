namespace GabinetePsicologia.Shared
{
    /// <summary>
    /// Tenant information
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// The tenant Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The tenant identifier
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// The tenant Name App
        /// </summary>
        public string NombreApp { get; set; }
        /// <summary>
        /// The tenant Database
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Tenant Css
        /// </summary>
        public string Css { get; set; }
   

        public Dictionary<string, object> Items { get; private set; } = new Dictionary<string, object>();

    }
}