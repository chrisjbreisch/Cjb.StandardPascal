program SalesTax (input, output); 
    {Asks for a price and amount tendered. Computes sales tax and change.} 
const TAXRATE =0.065;  {The local tax rate—6.5%.} 
      FIELD = 5; 
      DECIMALS = 2 ; {Print output to two decimal places.} 
var Price, AmountTendered, Tax, SalesPrice, Change: real;
begin 
    writeln ('Please enter the price.'); 
    readln (Price);
    Tax := TAXRATE * Price;
    SalesPrice := Price + Tax; 
    writeln ('The sales price is $', SalesPrice:FIELD:DECIMALS); 
    writeln ('What do you need change for?'); 
    readln (AmountTendered); 
    Change := AmountTendered-SalesPrice;
    writeln ('Your change is $', Change:FIELD:DECIMALS) 
end. {SalesTax}