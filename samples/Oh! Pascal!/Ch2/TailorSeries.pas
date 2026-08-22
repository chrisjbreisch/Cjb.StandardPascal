program TailorSeries (input, output); 
    {Demonstrates the assignment statement.} 
var NeckSize, HatSize, ShoeSize, ArmLength: real; 
    Waistline, Weight: integer; 
begin 
    writeln ('This program computes sizes for a male customer.'); 
    writeln ('Please enter the customers waistline, in inches.'); 
    readln (Waistline); 
    writeln ('Please enter the customer''s weight, in pounds.'); 
    readln (Weight); 
    NeckSize := 3.0 * (Weight /Waistline) ;
    HatSize := NeckSize / 2.125; 
    ShoeSize := 50.0 * (Waistline/Weight); 
    ArmLength := Waistline / 2.0; 
    writeln ('Neck size is ', NeckSize:2:2); 
    writeln ('Hat size is ', HatSize :2:2); 
    writeln ('Shoe size is ', ShoeSize:2:2); 
    writeln ('Arm length is ', ArmLength:2:2) 
end. {TailorSeries}