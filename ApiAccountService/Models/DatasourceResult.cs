namespace ApiAccountService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DatasourceResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int From { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Took { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }
    }
}
