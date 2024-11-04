using LightBDD.Core.Configuration;
using LightBDD.XUnit2;
using SpecificationTests.Contracts;

[assembly: ConfiguredLightBddScope] // the configured scope is applied on assembly
namespace SpecificationTests.Contracts;

internal class ConfiguredLightBddScopeAttribute 
    : LightBddScopeAttribute
{
    protected override void OnConfigure(LightBddConfiguration configuration)
    {
    }

    protected override void OnSetUp()
    {
        // additional code that has to be run before any LightBDD tests
    }

    protected override void OnTearDown()
    {
        // additional code that has to be run after all LightBDD tests
    }
}