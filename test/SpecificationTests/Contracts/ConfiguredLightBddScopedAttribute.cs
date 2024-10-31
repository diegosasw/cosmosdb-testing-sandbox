using LightBDD.XUnit2;
using SpecificationTests.Contracts;

[assembly: ConfiguredLightBddScope] // the configured scope is applied on assembly
namespace SpecificationTests.Contracts;

internal class ConfiguredLightBddScopeAttribute 
    : LightBddScopeAttribute;