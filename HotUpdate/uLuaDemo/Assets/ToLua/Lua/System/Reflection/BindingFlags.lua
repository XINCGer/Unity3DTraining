if System.Reflection == nil then    
    System.Reflection = {}
end

local function GetMask(...)
    local arg = {...}
    local value = 0 

    for i = 1, #arg do              
        value = value + arg[i]    
    end 
        
    return value
end

local BindingFlags = 
{
    Default = 0,
    IgnoreCase = 1,
    DeclaredOnly = 2,
    Instance = 4,
    Static = 8,
    Public = 16,
    NonPublic = 32,
    FlattenHierarchy = 64,
    InvokeMethod = 256,
    CreateInstance = 512,
    GetField = 1024,
    SetField = 2048,
    GetProperty = 4096,
    SetProperty = 8192,
    PutDispProperty = 16384,
    PutRefDispProperty = 32768,
    ExactBinding = 65536,
    SuppressChangeType = 131072,
    OptionalParamBinding = 262144,
    IgnoreReturn = 16777216,
}

System.Reflection.BindingFlags = BindingFlags
System.Reflection.BindingFlags.GetMask = GetMask

return BindingFlags