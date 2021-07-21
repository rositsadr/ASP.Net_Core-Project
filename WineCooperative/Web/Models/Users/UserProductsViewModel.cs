namespace Web.Models.Users
{
    public class UserProductsViewModel
    {
        public string Id { get; init; }   
        
        public string Name { get; init; }  
        
        public string ImageUrl { get; init; }  
        
        public string Description { get; init; } 
        
        public decimal Price { get; init; }      
        
        public string Color { get; init; }     
        
        public string Taste { get; init; }    
        
        public int ManufactureYear { get; init; }       

        public string Manufacturer { get; init; }    
        
        public bool InStock { get; init; }       
        
        public string WineArea { get; init; }
    }
}
