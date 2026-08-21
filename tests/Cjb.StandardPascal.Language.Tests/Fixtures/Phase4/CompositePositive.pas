program CompositePositive;
type
  IntPointer = ^integer;
  Point = record x: integer; end;
var
  values: array[1..2] of integer;
  point: Point;
  pointer: IntPointer;
begin
  values[1] := 3;
  point.x := values[1];
  new(pointer);
  pointer^ := point.x;
  writeln(pointer^);
  dispose(pointer);
end.