namespace TechStore.DTOs
{
    public class ProductFilterDto
    {
        public string? Search { get; set; }

        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        private int _page = 1;

        //Minimum 1'e Sabitliyoruz.
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value is < 1 or > 100 ? 10 : value;
        }

        //Page: 0/negatif gelirse otomatik 1'e sabitleniyor.
        //PageSize: 1'in altı ya da 100'ün üstü gelirse
        //(aşırı büyük istekleri engellemek için üst sınır 100), varsayılan olan 10'a düşüyor.
    }
}
