<img src="https://www.syntactik.com/img/Malina-logo.gif" alt="Malina Logo" />    

**MA**rkup **L**anguage **IN**tended for **A**ll

**This is a prototype of Syntactik ([www.syntactik.com](http://syntactik.com)) based on ANTLR parser generator. No further development of this language is planned because Syntactik is the final version.**

[![Join the chat at https://gitter.im/syntactik-Malina/Lobby](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/syntactik-Malina/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/8ehh57khcupl5nte?svg=true)](https://ci.appveyor.com/project/syntactik/malina)

# Overview
Malina is a people friendly markup language with code reuse features designed to be semantically compatible with XML and JSON formats.

- Define data using clean and intuitive syntax.
- Create aliases and reuse them as code fragments or document templates.
- Compile Malina documents into XML or JSON files.
- Validate Malina documents against XML schema.

Malina uses indents to define document structure similarly to Python and YAML. Inline and whitespace agnostic syntax is also available if it's needed to keep it short.

The purpose of the language is:
- to expand the audience of people working with data structures.
- to improve the productivity of individuals working with XML and JSON.
- creating new use cases by introducing the data-oriented markup language with people-friendly syntax and code reuse features.

# Language design requirements

Malina had the following initial requirements:
- It must be more people friendly than XML and JSON.
- It must be simpler than YAML.
- It must be semantically compatible with XML and JSON to replace them in editing (there is no intention to use Malina instead of XML and JSON in APIs).
- It must have powerful code reuse features.

# Language design principles

To meet the requirements mentioned above the following language design principles were used:
- Malina uses indents to define document structure (like Python, YAML, etc.). 
- Inline and whitespace agnostic syntax is also supported to satisfy needs of advanced users.
- Quotes are not required to define literals. Quotes should be used on special occasions like inline definitions, strings with special (escaped) symbols, string interpolation.
- Malina provides syntax to support basic primitives of both XML (like namespaces, elements, and attributes) and JSON (like collections and lists).
- Malina provides code reuse features similar to code reuse features of modern programming languages in the form of parameterized aliases and string interpolation.

# Example

```
//Namespace declarations
#ipo = http://www.example.com/ipo 
//"xsi" is a namespace prefix.
#xsi = "http://www.w3.org/2001/XMLSchema-instance" 

//Malina document called "PurchaseOrder". You can define several documents per file.
!PurchaseOrder: 
    ipo.purchaseOrder: //Xml element "purchaseOrder" with namespace prefix "ipo"
        @orderDate = "2016-12-20" //This is attribute
        shipTo:
            @xsi.type = ipo:EU-Address
            @export-code = 1
            ipo.name = Helen Zoe
            ipo.street = 47 Eden Street
            ipo.city = Cambridge
            ipo.postcode = 126            
        billTo:
            $Address.US.NJ //Alias with compound name
        Items:
            $Items.Jewelry.Necklaces.Lapis:
                .quantity = "2" //Argument of the alias
            item:
                @partNum = 748-OT
                productName = Diamond heart
                quantity = 1
                price = 248.90
                ipo.comment = Valentine's day packaging.
                shipDate = 2016-12-24
//Alias definitions                
!$Address.US.NJ:
    @xsi.type = ipo:US-Address
    ipo.name = Robert Smith
    ipo.street = 8 Oak Avenue
    ipo.city = Old Town
    ipo.state = NJ
    ipo.zip = 95819                

!$Items.Jewelry.Necklaces.Lapis:
    item:
        @partNum = 833-AA
        productName = Lapis necklace
        quantity = %quantity = "1" //Parameter with default value
        price = 99.95
        ipo.comment = Need this for the holidays!
        shipDate = 2016-12-12                
```
# Language reference

## Text encoding
- Currently the lexer supports **UTF-8**.
- Malina is **case sensitive**.
- Indents are defined by **whitespaces** formed by tab (**ASCII 0x9**, \t) or space (**ASCII 0x20**) symbols.
- Malina supports "Windows" (ASCII **0x0D0A**, \r \n) and "Linux" (ASCII **0x0A**, \n) **new line** symbols. 

## Comments

Malina supports single line comments. The comments start with **//**
```
//The whole line is a comment
name = "John Smith" //The rest of the line is a comment
```

## Name/Value pair
The **Name/Value pair** abstraction is a cornerstone of Malina language.
This abstraction consists of 3 parts:
- [Name](#name)
- [Assignment](#assignment)
- [Value](#value)

Any part could be omitted. For example omitted **name** or **value** means that the **name** or **value** is empty. The **assignment** can be only omitted if the **value** is omitted.

## Name
**Name** is a first part of the [Name/Value pair](#namevalue-pair). A **Name** starts with the [Prefix](#prefix) followed by the [Identifier](#identifier).

## Prefix
A **Prefix** defines a type of the language construct. 
If it is omitted, then the language construct is an [Element](#element).
This is the list of the prefixes with the corresponding language constructs:
 

| Prefix        | Type           |
| :-------------: |:-------------|
|      | [Element](#element) |
| `!`   | [Document](#document) |
| `!$` | [Alias Definition](#alias-definition)      |
| `#`      | [Namespace Declaration](#namespace-declaration) or [Namespace Scope](#namespace-scope) |
| `@`  | [Attribute](#attribute)   |
| `$` | [Alias](#alias) |
| `.` | [Argument](#argument) |
| `%` | [Parameter](#parameter) |

## Identifier

In the current version, **IDs** are defined by the same rules as for an XML element name:
- Element id is case-sensitive
- Element id must start with a letter or underscore
- Element id can contain letters, digits, hyphens, underscores, and periods
- Element names cannot contain spaces
- See the full spec for the XML Name here http://www.w3.org/TR/REC-xml/#NT-Name

>JSON has different rules for names. In the future versions, for the sake of the full semantic compatibility with JSON, the support of JSON names will be added by the introduction of quoted IDs. Still, the current implementation covers the majority of the use cases because names with spaces or other special symbols are rarely used in real life scenarios.

### Compound Identifier
ID with the dot(s) is called a **Compound Identifier**. Dots split a compound identifier into several parts.
In the [Element](#element), [Attribute](#attribute) or [Namespace Scope](#namespace-scope) the **compound identifier** is used to specify [namespace prefix](#namespace-prefix). 
For example, in the following code, the attribute and element have the namespace prefix `ipo`.
```
@ipo.export-code = 1
ipo.name = Robert Smith
```
All language constructs except a [Namespace Declaration](#namespace-declaration) can have a Compound Identifier.
The compound IDs are used in [Alias definitions](#alias-definition) to structure them in the same way classes are structured using namespaces in programming languages:
```
!$Items.Jewelry.Necklaces.Lapis:
...
!$Items.Jewelry.Necklaces.Pearl:
...
!$Items.Jewelry.Diamonds.Heart:
...
!$Items.Jewelry.Diamonds.Uncut:
...
!$Items.Jewelry.Rings.Amber:
...
!$Items.Jewelry.Earings.Jade:
...
```
>Code editors can take advantage of compound IDs to provide a better autocomplete.

### Simple Identifier
The term **Simple Identifier** is used as the opposite to the  [Compound Identifier](#compound-identifier), meaning that the identifier doesn't have any dots in it. Thus it is not divided into parts and represents a single word identifier.
In the example below the `name` is a simple identifier:
```
name = John Smith
```

## Value
There are two types of values: **literal** and **object**.

### Literal
In Malina, **literals** have 4 types: 
- **strings** are the only literals used in XML. In addition to string literals, JSON also uses: 
- **number**, 
- **boolean** and 
- **null literals**. 

The **empty literal** is defined by omitted [value](#value) and represents the **empty string**.

### Object
**Object** is a set of name/value pairs. A set of name/value pairs is defined in Malina using [Blocks](#block).

### Empty Object
The empty objects is a special type of value which, being an [object](#object), doesn't contain any name/value pairs.
The [empty block](#empty-block) represents the **empty object**.

## Block
Block is a syntax construct for defining an [object](#object). 
As mentioned in the section ["language design principles"](#language-design-principles), Malina uses indents to define document structure. The lines of code, having the same indentation, belong to the same block.
>A line with the greater indent still indirectly belongs to the same block because it is a part of a nested block.

In the following example, the element `billTo` has a block with one attribute and five elements.
```
billTo:
    @xsi.type = "ipo:US-Address"
    ipo.name = Robert Smith
    ipo.street = 8 Oak Avenue
    ipo.city = Old Town
    ipo.state = AK
    ipo.zip = 95819
```
## Empty Block
Block can be empty. In this case it represents the [empty object](#empty-object).
In the example below, the element `items` has the [empty object value](#empty-object).
```
items:
comments = this is the empty order
```

## Assignment
Malina has two types of an assignment operator: **literal assignment** and **object assignment**.

### Literal Assignment
The **literal assignment** is used when the right side of the assignment is a [literal](#literal). Equality sigh `=` is used for the assignment of a literal:
```
name = John Smith
```
>In some cases double equality `==` is used for the assignment of a literal. This particular cases are called [free open string](#free-open-string) and [folded open string](#folded-open-string).


### Object Assignment
The **object assignment** is used when the right side of the assignment is an [object](#object). Colon `:` is used for the assignment of an object value:
```
address:
    street = 1100 Main St
    city = Prescott
    state = AZ
```
In the example above, the element `address` has an [object value](#object) which consists of three elements: `street`, `city` and `state`.

> With the two types of assignments, Malina resolves syntax ambiguities and, at the same time, improves readability of code.


## Module
Malina has a notion of a **Module**. 
Module is a container for:
- [Namespace declarations](#namespace-declaration)
- [Documents](#document)
- [Alias Definitions](#alias-definition)

A **Module** is physically represented by a file with the extentions **"mlx"** or **"mlj"**.

### Mlx-module

**Mlx-module** is physically represented by a file with the extentions **"mlx"**. **Mlx-module** contains [documents](#document) symantically representing XML documents. 

### Mlj-module

**Mlj-modules** is physically represented by a file with the extentions **"mlj"**. **Mlj-module** contains [documents](#document) symantically representing JSON documents.

The differences in the MLX and MLJ semantics are covered later in this document.

## Namespace Declaration
### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `#` |
| [Identifier](#identifier)    | [simple](#simple-identifier) |
| [Value](#value) | [literal](#literal)      |
| Declared in | [Module](#module), [Document](#document), [Alias Definition](#alias-definition) |

### Description

Namespaces can be declared in the [Module](#module), [Document](#document) or [Alias Definition](#alias-definition).
The hash symbol `#` starts the declaration of a namespace followed by a name/value pair, there an [identifier](#identifier) represents an [xml namespace prefix](https://www.w3.org/TR/xml-names11/#NT-Prefix) and a [string literal](#literal) represents an [xml namespace name](https://www.w3.org/TR/xml-names11/#dt-NSName).
For example:
```
#xsi = http://www.w3.org/2001/XMLSchema-instance
#ipo = http://www.example.com/myipo
```

### Namespace Prefix
A **namespace prefix** is a [simple identifier](#simple-identifier).
The namespace declared in the module is visible in the all documents and alias definitions in this module. The namespace declared in the document and alias definitions is visible only inside the document or alias definition.  The namespace declared in the document or alias definition overrides the namespace with the same namespace prefix declared in the module.

## Document

### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `!` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal)      |
| Declared in | [Module](#module) |
| Contains | [Namespace Declaration](#namespace-declaration), [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope) |

### Description

Document starts with the exclamation mark `!` followed by [Identifier](#identifier), colon **:** and [object](#object).
```
!PurchaseOrder: // Exclamation mark “!” followed by the identifier "PurchaseOrder" and colon : 
    ipo.purchaseOrder: //object
        @orderDate = 1999-12-01
        shipTo:
            @xsi.type = ipo:EU-Address
            @export-code = 1
            ipo.name = Helen Zoe
            ipo.street = 47 Eden Street
            ipo.city = Cambridge
            ipo.postcode = 126
```
### Mlx-document
An **Mlx-document** represents the XML file. It must be defined in the [**mlx-module**](#module).
Mlx-document must have only one root element. 

### Mlj-document
A **Mlj-document** represent then JSON file. It must be defined in [**mlj-modules**](#module).

Because in JSON the literal value represents the valid document, **Mlj-document** name can be also followed by equality `=` and [literal value](#literal).
```
!document = This is a valid json document.
```

## Element

### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | none |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Parameter](#parameter) |

### Description

An Element is the most used type of a name/value pair in Malina. It corresponds to XML element and a name\value pair in a JSON object. 
Element [name](#name) doesn't have a [prefix](#prefix).
An element name can be a [compound identifier](#compound-identifier). In this case first part of the name is a [namespace prefix](#namespace-prefix). The [namespace prefix](#namespace-prefix) has to be declared in the [module](#module), [document](#document) or [alias definition](#alias-definition). 
```
ipo.name = Robert Smith
```
If the [assignment](#assignment) and [value](#value) are omitted then the element has the [empty object value](#empty-object).

## Attribute
### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `@` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal)    |
| Declared in | [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |

### Description
An **attribute** corresponds to an attribute in XML and a name\value pair in a JSON object.
[Mlj-documents](#mlj-document) should not have attributes, but it is allowed to use them. In this case, it will be treated as the [element](#element) with the same name.
An attribute starts with "at sign", `@`, followed by [identifier](#identifier) that can be [compound](#compound-identifier). 
In the compound identifier, the first part of the name is a namespace prefix. The namespace prefix has to be declared in the [module](#module), [document](#document) or [alias definition](#alias-definition).
The **attribute** can have only the [literal value](#literal). If the [assignment](#assignment) and [value](#value) are omitted then the attribute has the **empty string value**.
```
@orderDate = 2016-12-01
@ipo.export-code = 1
@emptyString = 
@empty
```

## Alias
### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `$` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Argument](#argument), [Parameter](#parameter) |

### Description
Malina has еру powerful code reuse feature called **Alias**. An Alias is a short name for a fragment of code.
An alias starts with a dollar sign `$` followed by a [simple identifier](#simple-identifier) or [compound identifier](#compound-identifier). 
In some cases a [value](#value) and [assignment](#assignment) are omitted in the **alias**. In other cases the **alias**  has the assignment and the value in the form of a [block of arguments](#block-of-arguments) or [default argument](#default-argument).

[Alias Definition](#alias-definition) has to define an alias. It is recommended to use [compound identifiers](#compound-identifier) to create a tree structure of aliases.
Depending on the code it represents, the alias can be either a [literal alias](#literal-alias) or an [object alias](#object-alias).

### Literal Alias
A **Literal Alias** represent a [literal](#literal) and can be used in place of the literal value or inside of the [string interpolation](#string-interpolation).
```
// Alias CurrentDate has the simple identifier and the literal value
@orderDate = $CurrentDate

// Alias $customer.firstName has the compound identifier. 
// It is used inside the string interpolation.
@customerGreating = 'Hello $customer.firstName'
```
### Object Alias

An **Object Alias** represents an object that have only the following types of [name/value pairs](#namevalue-pair):

- [element](#element)
- [attribute](#attribute)
- [alias](#alias). 

An **Object Alias** can represent either a whole object or its part. It can also represent an [empty object](#empty-object).

Example:
```
items:
    //This is the list of aliases with the compound identifiers and object values
    $Items.Jewelry.Necklaces.Lapis
    $Items.Jewelry.Necklaces.Pearl
    $Items.Jewelry.Diamonds.Heart
    $Items.Jewelry.Diamonds.Uncut
    $Items.Jewelry.Rings.Amber
    $Items.Jewelry.Earings.Jade
```
## Alias Definition
### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `!$` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Module](#module) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope),  [Parameter](#parameter) |

### Description
[Aliases](#alias) are defined in the [module](#module). An **Alias Definition** starts with the exclamation and dollar signs `!$` followed by an [Identifier](#identifier).

### Object Alias Definition 
### Literal Alias Definition
Depending on the code it represents, the **alias definition** declares either a [literal alias](#literal-alias) or an [object alias](#object-alias).
In the example below, the alias definition declares the object alias `$Address.US` and the literal alias `$Pi`:
```
//Object Alias Definition
!$Address.US:
    @xsi.type = ipo:US-Address
    ipo.name = Robert Smith
    ipo.street = 8 Oak Avenue
    ipo.city = Old Town
    ipo.state = AK
    ipo.zip = 95819

//Literal Alias Definition
!$Pi = 3.14159265359
```


## Parameter
### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `%` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Attribute](#attribute), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope),  [Parameter](#parameter) |

### Description
[Alias Definition](#alias-definition) can have **parameters** and then it's called a **parameterized alias definition**.
A parameter can represent either an [object](#object) or [literal](#literal) value, thus there are two types of parameters: [object parameter](#object-parameter) and [literal parameter](#literal-parameter). 
A parameter starts with percent sign `%` followed by an [Identifier](#identifier). Two or more parameters with the same name are allowed if they have they have the same value type: [object](#object) or [literal](#literal). 

### Object parameter
**Object parameter** represents an object (the whole object or some of the object's name/value pairs).
In the example below the alias definition `!$Templates.PurchaseOrder.With.Necklace.Lapis` has 3 object parameters: `%shipTo`, `%billTo` and `%items`.
```
!$Templates.PurchaseOrder.With.Necklace.Lapis:
    purchaseOrder:
        shipTo:
            %shipTo
        billTo:
            %billTo
        Items:
            $Items.Jewelry.Necklaces.Lapis
            %items
```
Parameters `%shipTo` and `%billTo` represent the whole object. The parameter `%items` represents the trailing part of the object where the leading part of the object is defined by the alias `$Items.Jewelry.Necklaces.Lapis`.


### Literal parameter
**Literal parameter** represents a  whole or partial literal value. 
In the example below, the alias definition `!$Address.US.NJ` has 2 literal parameters: `%name` and `%street`. Both parameters represent the whole literal value. The literal parameter `%customerName` defined in the [literal alias definition](#literal-alias-definition) `!$CustomerGreating` represents a part of the literal value. The parameter `%customerName` is used inside of the [string interpolation](#string-interpolation) `'Hello %customerName'`.
```
!$Address.US.NJ:
    @xsi.type = ipo:US-Address
    ipo.name = %name
    ipo.street = %street
    ipo.city = Fort Lee
    ipo.state = NJ
    ipo.zip = 07024
    
!$CustomerGreating = 'Hello %customerName'
```

### Default Parameter Value
All types of parameters can have a default value.
For the [object parameter](#object-parameter) the default value is defined by the rules of the [object assignment](#object-assignment).
In the example below, the parameter `%shipTo` has the default object value defined by the alias `$Address.UK.Cambridge` and the parameter `%billTo` has the default object value defined by [block (object)](#block) which consists of 5 elements. The parameter `%items` has the the [empty object](#empty-object) as a default value.
```
!$Templates.PurchaseOrder.International:
    purchaseOrder:
        shipTo:
            %shipTo:
                $Address.UK.Cambridge
        billTo:
            %billTo:
                name = Robert Smith
                street = 8 Oak Avenue
                city = Old Town
                state = AK
                zip = 95819
        Items:
            %items:
```
For the [literal parameter](#literal-parameter) the default value is defined by the rules of the [literal assignment](#literal-assignment).
In the following example, the parameter `%name` has the default string value `John Smith`, the parameter `%street` has the default value defined by the alias `$DefaultStreetName, ` and the parameter `%customerName` has the default value `"My Friend"`.

```
!$Address.US.NJ:
    @xsi.type = ipo:US-Address
    ipo.name = %name = John Smith
    ipo.street = %street = $DefaultStreetAddress
    ipo.city = Fort Lee
    ipo.state = NJ
    ipo.zip = 07024
    
!$CustomerGreating = 'Hello %(customerName = "My Friend")'
```
### Default Parameter
### Default Argument
If the [Alias Definition](#alias-definition) has only one [parameter](#parameter) then this parameter is called a **Default Parameter**. In the corresponding [alias](#alias) you don't need to specify [argument](#argument) for the **default parameter**. Instead, the value of the argument has to be assigned directly to the alias. If the **default parameter** is an [object parameter](#object-parameter) then [object value](#object) has to be assigned to the alias. And the same should be done for the [literal parameter](#literal-parameter) amd [literal value](#literal).

In the example below:
+ the alias definition `!$CustomerGreating` has default literal parameter `%customerName` which has a default value `"My Friend"`
+ the alias definition `!$Bold` has the default object parameter `%body`
+ the alias `$Bold`, in the document `!htmlDocument`, has the default object argument `span = $CustomerGreating = Robert Smith`
+ the alias `$CustomerGreating`, in the alias `$Bold`, has default literal argument `Robert Smith`

```
!$CustomerGreating = 'Hello %(customerName = "My Friend")!'

!$Bold:
    b:
        %body

!htmlDocument:
    html:
        body:
            $Bold:
                span = $CustomerGreating = Robert Smith
```


## Argument
### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `.` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Alias](#alias) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope),  [Parameter](#parameter) |

### Description
The [Alias](#alias) can have **arguments** if its [definition (alias definition)](#alias-definition) has [parameters](#parameter). An argument can represent either [object](#object) or [literal](#literal) value, thus there are two types of arguments: [object argument](#object-argument) and [literal argument](#literal-argument). 
An argument starts with dot `.` followed by an [Identifier](#identifier).
Each argument corresponds to the parameter with the same name. There are several restrictions for arguments:

1. The argument must have the same [value type](#value) as the corresponding parameter.
2. There can't be two or more arguments with the same name.
3. If the parameter doesn't have a default value, then the argument with the same name must be specified in the alias.
4. If the parameter does have a default value, then the corresponding argument can be omitted.

All arguments are passed to the alias as an [object value](#object) in the form of a [Block of Arguments](#block-of-arguments). 


### Block of Arguments
Block of Arguments is a [block](#block) where each [name/value pair](#namevalue-pair) is an [argument](#argument). A block of arguments always represents an [object value](#object) of [alias](#alias).

### Object Argument
An **Object Argument** is used to specify the value of an [object parameter](#object-parameter) in the [alias](#alias).
In the example below, the alias `$Templates.PurchaseOrder` has the [block of arguments](#block-of-arguments) which consists of 3 **object arguments**: `.shipTo`, `.billTo` and `.items`. 
```
!PurchaseOrderFromTemplate:
    $Templates.PurchaseOrder:
        .shipTo:
            $Address.UK.Cambridge
        .billTo:
            name = Robert Smith
            street = 8 Oak Avenue
            city = Old Town
            state = NJ
            zip = 95819
        .items:
            $Items.Jewelry.Necklaces.Lapis
            $Items.Jewelry.Diamonds.Heart
```
### Literal Argument
The **Literal Argument** is used to specify the value of the [literal parameter](#literal-parameter) in the [alias](#alias).
In the following example, the alias `$$Items.Jewelry.Necklaces.Lapis` has the [block of arguments](#block-of-arguments) which consists of 1 **literal arguments**: `.quantity` with the assigned [literal](#literal) value of `2`. 
```
...
    Items:
        $Items.Jewelry.Necklaces.Lapis: 
            .quantity = 2
...
```

## Namespace Scope

### Overview
| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `#` |
| [Identifier](#identifier)    | [simple](#simple-identifier), [compound](#compound-identifier) or [omitted](#empty-namespace-scope) |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Parameter](#parameter) |

### Description

A **Namespace Scope** in Malina has the same purpose as a [default namespace](https://www.w3.org/TR/REC-xml-names/#defaulting) in XML. Basically, it defines the namespace for the [elements](#element) that have no [namespace prefix](#namespace-prefix).
A **Namespace Scope** starts with the hash symbol `#` followed by [simple](#simple-identifier) or [compound identifier](#compound-identifier).
The **simple identifier** represent the [namespace prefix](#namespace-prefix) (*it has to be declared previosly in the [module](#module), [document](#document) or [alias definition](#alias-definition)*). The [object  assignment](#object-assignment) follows the identifier. All elements inside the [object value](#object) that don't have a [namespace prefix](#namespace-prefix) will got the **default namespace prefix** from the scope.
In the following example, the scope `#ipo` defines the default namespace for all elements in its [object value](#object).
```
...
    #ipo:
        purchaseOrder:
            shipTo:
                name = Helen Zoe
                street = 47 Eden Street
                city = Cambridge
                postcode = 126
...
```
A scope with a **compound identifier** is used to declare a **default namespace prefix** and an element at the same time. The first part of the [compound identifier](#compound-identifier) represents the **default namespace prefix** and the rest represent the name of the element. 
The example below uses the namespace scope with the [compound identifier](#compound-identifier). The scope `#ipo.purchaseOrder` declares the **default namespace prefix** `ipo` and the element `purchaseOrder`. This example is semantically identical to the example above.

```
...
    #ipo.purchaseOrder:
            shipTo:
                name = Helen Zoe
                street = 47 Eden Street
                city = Cambridge
                postcode = 126
...
```
The **Namespace Scope** impacts only element that are defined inside its [object-value](#object). It doesn't impact elements defined inside the [alias](#alias).
**Namespace Scopes** can be nested. The inner scope override the action the outer scope.

### Empty Namespace Scope
Sometimes it's needed to set the value of the default namespace prefix to empty. So there will be no default namespace prefix. In this case, the name of the default namespace prefix should be omitted when defining the **namespace scope**. For example: 
```
...
    #:
        purchaseOrder:...
```
**or**
```
...
    #.purchaseOrder:
...
```
## JSON Array
In Malina, **JSON Array** is a special case of an [object](#object). Like an [object](#object), a **JSON Array** is defined using a [block](#block). 
To be a **JSON Array**, the [object](#object) has to comply with the condition that all of its [name/value pairs](#namevalue-pair) have no name. This kind of the [name/value pairs](#namevalue-pair) is called an [Array Item](#array-item).

### Array Item

#### Overview

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | none |
| [Identifier](#identifier)    |  omitted |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Declared in | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Contains | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Parameter](#parameter) |

#### Description
An **Array Item** is a [name/value pairs](#namevalue-pair) that have no name.
There are 2 types of the array item: [Literal Array Item](#literal-array-item) and [Object Array Item](#object-array-item).

#### Literal Array Item
A **literal array item** consists of an empty name, a [literal value assignment](#literal-assignment) and an optional [literal value](#literal-value). If the [literal value](#literal-value) is omitted then the item represents the empty string.
In the example below, the element `colors` has the value defined as the **array** of 7 **literal array items**.
```
colors:
    = red
    = orange
    = yellow
    = green
    = blue
    = indigo
    = violet
```
#### Object Array Item
An **object array item** consists of an empty name, an [object value assignment](#object-assignment) and an optional [object value](#object). If the [object value](#object) is omitted the the item represents the [empty object](#empty-object).
In the example below, the element `Items` has the value defined as the **array** of 3 **object array items**.
```
Items:
    :
        productName = Lapis necklace
        quantity = 2
        price = 99.95
        ipo.comment = Need this for the holidays!
        shipDate = 1999-12-05
    :
        productName = Diamond heart
        quantity = 1
        price = 248.90
        ipo.comment = Valentine's day packaging.
        shipDate = 2000-02-14
    :
        productName = Uncut diamond
        quantity = 7
        price = 79.90
        shipDate = 2000-01-07
```

### String Literals

#### Overview
Malina has two types of string literals:
+ **quoted string literals** use single or double quotes to define a literal
+ **open string literals** do not use quotes to define a literal

Each type has 2 subtypes. **Quoted string literals** include [single quoted strings](#single-quoted-string) and [double quoted strings](#double-quoted-string). **Open string literals** include [open string](#open-string) and [free open string](#free-open-string).

There is also a **multiline** version of each subtype, so there are 8 variations of **string literals**:
+ [open string](#open-string)
+ [free open string](#free-open-string)
+ [double quoted string](#double-quoted-string)
+ [single quoted string](#single-quoted-string)
+ [multiline open string](#multiline-open-string)
+ [folded open string](#folded-open-string) (multiline version of [free open string](#free-open-string))
+ [multiline double quoted string](#multiline-double-quoted-string)
+ [multiline single quoted string](#multiline-single-quoted-string)

### Open String
An **Open String** defines a literal without using quotes. The equality sign `=` starts the **open string** followed by the symbols representing the [literal value](#literal-value). If the [literal value](#literal-value) is omitted then the **open string** represents the **empty string**.
In the example below all literals are defined by the **open strings**:
```
productName = Diamond heart 
quantity = 1 
price = 248.90 
ipo.comment = 
shipDate = 2000-02-14
```

There are several restrictions for the **open strings**:
+  it can’t start from the dollar sign `$` or the percent sign `%` followed by an [identifier](#identifier) because it actually means that the value is a [literal alias](#literal-alias) or [literal parameter](#literal-parameter).
+  it can’t start from single `'` or double quote `"` because this how [quoted string](#quoted-string) starts.
+  parser ignores leading and trailing whitespaces
+  it can’t have any special non-visible symbols that require escaping (like newline)

Malina has the second **open string** called [free open string](#free-open-string) that allows avoiding the first two restrictions.
There is the multiline version of the **open string** called [multiline open string](#multiline-open-string).

### Free Open String
A **Free Open String** defines a literal without using quotes like [open string](#open-string) but with fewer restrictions. 
There are just two constraints for the **free open strings**:
+  the parser ignores leading and trailing whitespaces
+  it can’t have any non-visible symbols that require escaping (like newline)

The double equality sign `==` starts the **free open string** followed by the symbols representing the [literal value](#literal-value). If the [literal value](#literal-value) is omitted then the **free open string** represents the **empty string**.
In the example below all literals are defined by the **free open strings**.
```
fileName == $random_seed$
factOfTheDay == "Stranger Things" is a science fiction-horror television series.
```
There is the multiline version of the **free open string** called [folded open string](#folded-open-string).

### Double Quoted String
A **Double Quoted String** defines a literal using double quotes `"`.
A **Double Quoted String** can’t have any special non-visible symbols that require escaping (like newline).
A double quote `"` inside a **double quoted string**  can be escaped with the two consecutive double quotes `""`.
In the following example, all literals are defined using a **double quoted string**.
```
dirName = "c:\Windows"
factOfTheDay = "I like ""Strange' Things"" science fiction-horror television series."
```

There is the multiline version of the **double quoted string** called [multiline double quoted string](#multiline-double-quoted-string).

### Single Quoted String
A **Single Quoted String** defines a literal using single quotes `'`.
The two single quotes `''` escape the single quote `'` inside a **single quoted string**.
A **single quoted string** can include [escape sequences](https://en.wikipedia.org/wiki/Escape_sequence#Programming_languages). 

#### Escape Sequences
A [single quoted string](#single-quoted-string) and [multiline single quoted string](#multiline-single-quoted-string) support Java script escape sequences:

| Unicode character value        | Escape sequence           | Meaning  |
| :-------------: |:-------------:|:-------------:|
| `\u0008`     | `\b` | Backspace |
| `\u0009` | `\t` | Tab |
| `\u000A` | `\n` | Line feed (new line) |
| `\u000B` | `\v` | Vertical tab |
| `\u000C` | `\f` | Form feed |
| `\u000D` | `\r` | Carriage return |
| `\u0022` | `\"` | Double quotation mark `"` |
| `\u0027` | `\'` | Single quotation mark `'` |
| `\u005C` | `\\` | Backslash `\`

Malina also has own escape sequences. Malina escape sequence starts with the dollar sign `$` followed by a decimal number or hash sign `#` and hexadecimal number where the number represents a UTF-8 symbol. The brackets `()` can surround the decimal or hexadecimal number. The max length of decimal and hexadecimal number is 5 and 4 digits.
The following example shows how to escape new line symbol using Malina escape sequence.

| Use Case        | decimal           | hex  |
|:-------------: |:-------------:|:-------------:|
| Escape symbol not followed by digit (decimal or hex)     | `$10` | `$#A` |
|Escape symbol followed by a digit (decimal or hex). Max length is 6 symbols including `$` or `$#`.| `$(10)`, `$00010` | `$(#A)`, `$#000A`|

It is recommended to use brackets to explicitly define start and end of the number and avoid mistakes.

The dollar sign `$` and percent sign `%` have a special meaning inside a **single quoted string**. The symbols can be escaped as `$$`, `\$` and `%%`, `\%`.

#### String Interpolation 
A **single quoted string** supports [string interpolation](https://en.wikipedia.org/wiki/String_interpolation).  **String interpolation** allows you to insert a value of an alias or parameter inside the string. **String interpolation** starts with `$` (for an alias) or `%` (for a parameter) inside the **single quoted string** followed by [identifier](#identifier) of the alias or parameter. The parentheses `()` can surround the identifier.  The parentheses are optional if the interpolation is followed by space or another symbol that can’t be interpreted as part of the alias or parameter name.

Example:

```
line1 = 'The customer name is %CustomerName' 
line2 = 'The customer\'s phone number is $(Phone.AreaCode)-$(Phone.LocalNumber)-$Phone.Extention'
```

### Multiline Open String
**Multiline Open String** is an easy way to add long strings to your Malina document. It uses indents to define multiline strings. For example:
```
LoremIpsum = Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque a lectus gravida, congue. 
    Quisque mollis ut odio sed facilisis. Donec dictum ullamcorper lectus, ultrices convallis justo. 
```
The second, third, etc. lines must be indented by exactly the one whitespace. Extra indents will be included in the string.
The leading whitespaces in the first line and trailing spaces of the last line will be ignored.

If the first line doesn’t have any symbols, then it will be ignored, and the string will start from the next line.
The ending double equality `==` is used to specify the end of the string. If it's omitted, the string will end based on indents, and trailing white space and new line symbols will be ignored. The ending double equality must have the same indent as the name of the current [name/value pair](#namevalue-pair). 

```
LoremIpsum = 
    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque a lectus gravida, congue. 
    Quisque mollis ut odio sed facilisis. Donec dictum ullamcorper lectus, ultrices convallis justo. 
==
```

### Folded Open String
Sometimes strings are too long and, therefore, are hard to read and edit. The **folded open string** can help in this case. **Folded open string** follows the double equality `==`. In the folded open string, a single new line becomes a space. Two consecutive newlines are treated as one; three consequent newlines are treated as two etc. For example:
```
LoremIpsum ==
    Lorem ipsum dolor sit amet, consectetur adipiscing elit.  
    Pellentesque a lectus gravida, congue tellus quis, sagittis massa.  
    Aenean sed odio nec tellus vulputate vestibulum. Sed sed pulvinar  
    ex. 

    Quisque mollis ut odio sed facilisis. Donec dictum ullamcorper  
    lectus, ultrices convallis justo volutpat at. Cras nibh tellus, placerat  
    in pretium sit amet, tempor sit amet nibh. Nulla tempus molestie ligula  
    sed semper.
==
```
Please note that in the example above the first line is empty and, therefore, ignored. Also, the output string will end with new line symbol because double equality `==` is used to end the **folded open string**.

### Multiline Double Quoted String
A **Multiline Double Quoted String** can be used to define multiline strings. It works pretty straightforward. The first line of the string starts after the first double quote. The second and the next lines of the string must be indented. The string ends with the closing double quote. A double quote `"` inside a **multiline double quoted string**  can be escaped with the two consequent double quotes `""`. 
```
LoremIpsum = "Lorem ipsum dolor sit amet, consectetur ""adipiscing"" elit. Pellentesque a congue. 
    Quisque mollis ut odio sed facilisis. Donec dictum ullamcorper lectus, convallis volutpat at"
```
### Multiline Single Quoted String
A **Multiline Single Quoted String** allows usage of the [string interpolation](#string-interpolation) and [escape sequences](#escape-sequences) in the multiline string.  At the same time, it works with new lines like [folded open string](#folded-open-string).

```
LoremIpsum = '\tLorem ipsum dolor $CustomerName consectetur adipiscing elit.     
    Pellentesque a lectus gravida, congue tellus quis, sagittis massa.  
    Aenean sed odio nec tellus vulputate vestibulum. Sed sed pulvinar  
    ex. \n 
        Quisque mollis ut odio sed facilisis. Donec dictum ullamcorper  
    lectus, ultrices convallis justo volutpat at. Cras nibh tellus, placerat  
    in pretium sit amet, tempor sit amet nibh. Nulla tempus molestie ligula'
```
### JSON Literals
Besides **strings**, JSON recognises **numbers** and literals `true`, `false`, `null`. To be semantically compatible with JSON, Malina automatically recognises **JSON literals** in [open strings](#open-string) and [single quoted string](#single-quoted-string) inside [mlj-documents](#mlj-documents). If **JSON literal** is in [double quoted string](#double-quoted-string) then it will be treated as a regular string. Malina uses the original format of the [JSON literals](http://www.json.org/). 
The example below shows cases when the literal is recognised as a **JSON literal** or a **string**:
```
number = 123
number = '123'
string = "123"
true = true
true = 'true'
string = "true"
string == true
false = false
null = 'null'
```
## Inline Syntax
Inline syntax allows to define several language constucts in one line of code. There are several use cases when you can use the **inline syntax** which are covered in the sections below.

### Inline Definitions 
Two or more [name/value pairs](#namevalue-pair) can be defined in the same line. In this case, they are treated like they are defined in separate lines with the same indent. The first example below shows regular, "one definition per line", way to define a [block](#block) of [elements](#element). The second example shows the example of the inline syntax used to define the same [block](#block) of [elements](#element).

```
name = Helen Zoe
street = 47 Eden Street
city = Cambridge
postcode = 126
```
```
name = "Helen Zoe" street = "47 Eden Street" city = "Cambridge" postcode = 126
```
In the second example the [quoted strings](#double-quoted-string) are used to define the [string literals](#string-literals). The usage of the [quoted strings](#double-quoted-string) instead of [open strings](#open-string) is important in this case, because it is possible for the parser to determine where the one name/value pair ends and another one begins. 
The [single quoted strings](#single-quoted-string) could be used as well. The usage of  [single quoted strings](#single-quoted-string) is mandatory when you need to define a [JSON literal](#json-literal) in the **inline definition**.
Please also note that the last name/value pair in the second example is still using the [open string](#open-string). It doesn't cause any issue because this is the last name/value pair in the line.
> Basically, you can define a [name/value pair](#namevalue-pair) at the same line with the previous [name/value pair](#namevalue-pair) if both pairs belong to the same [block](#block) and a **quoted string** was used to define value of the previous [name/value pair](#namevalue-pair).

### Inline Block
**Inline Block** is a [block](#block) that is defined in the one line with the [name](#name) and the [object assignment](#object-assignment) `:`.
The first example below shows the element `shipTo` with the regular block definition. The second example shows the same data structure defined using the **inline block** syntax.

```
shipTo:
    name = Helen Zoe
    street = 47 Eden Street
    city = Cambridge
    postcode = 126
```
```
shipTo: name = "Helen Zoe" street = "47 Eden Street" city = "Cambridge" postcode = '126'
```
### Nested Inline Blocks
**Nested Inline Block** are the nested [blocks](#block) defined in the one line. Any [name/value pair](#namevalue-pair) that is already in the inline block can start its own [inline block](#inline-block). And any [name/value pair](#namevalue-pair) that is already in the inline block belongs to the last declared block.
In the following example, elements `name`, `street`, `city` and `postcode` are defined in the [inline block](#inline-block) that belongs to the element `shipTo` which itself belong to the [inline block](#inline-block) of the element `purchaseOrder`.
```
!PurchaseOrder:
    purchaseOrder: shipTo: name = "Helen Zoe" street = "47 Eden Street" city = "Cambridge" postcode = 126        
```
Comma `,` is used to end the current [inline block](#inline-block). It allows creation the **nested inline block** of any topology.
In the first example below, the nested blocks are defined by indents. The second example defines the same data structure using the **nested inline block** syntax with commas `,`.
```
root:
    el1:
        el1_1 = text1_1
        el1_2 = text1_2
    el2:
        el2_1 = text2_1
        el2_2 = text2_2
    el3:
        el3_1 = text3_1
        el3_2 = text3_2        
```
```
root: el1: el1_1 = "text1_1" el1_2 = "text1_2", el2: el2_1 = "text2_1" el2_2 = "text2_2"
```

If needed, the readability of the inline block can be improved with parentheses `()`:

```
root: el1:(el1_1 = "text1_1" el1_2 = "text1_2"), el2:(el2_1 = "text2_1" el2_2 = "text2_2")
```
Parentheses `()` are also used to start and finish the [WSA region](#wsa-mode), although the example above do not use [WSA](#wsa-mode) features of the language, but only use parentheses to improved readability.

## WSA Mode
**WSA** is an acronym for **Whitespace Agnostic Mode**. It is a mode when the Malina parser ignores indents and dedents.
Parentheses `()` define the **WSA region**. Parentheses could be nested. Parser quits the **Whitespace Agnostic Mode** only when the number of closing parentheses `)` is not less than the number of opening parentheses `(`.
In the first example below, the blocks are defined by indents. The second example defines the same data structure using the **Whitespace Agnostic Mode**:

```
root:
    el1:
        el1_1 = text1_1
        el1_2 = text1_2
        el1_3 = text1_3
        el1_4 = text1_4
    el2:
        el2_1 = text2_1
        el2_2 = text2_2
```
```
root:
    el1:(
                el1_1 = text1_1
    el1_2 = text1_2
            el1_3 = "text1_3" el1_4 = text1_4
    )
    el2: (
        el2_1 = text2_1
            el2_2 = text2_2
        )
```
The [string literals](#string-literals) ignore indents and dedents when they are defined in a **WSA region**. So it is not possible to define multiline literal inside the parentheses.

## XML Mixed Content
Malina support XML elements with [mixed content](#https://www.w3.org/TR/REC-xml/#sec-mixed-content). The **mixed content** means that XML element's content has at least one text node and at least one element or an attribute. 
In Malina, in case of the **mixed content**, text nodes has to be included in the [element's](#element) object value in a form of [literal array item](#literal-array-item). 
> Although mlx-documents do not support [arrays](#json-array), a [literal array items](#literal-array-item) can be included in the element's object value to represent text nodes.

As an example we can use the following XML:
```xml
<letter>
  Dear Mr.<name>John Smith</name>.
  Your order <orderid>1032</orderid>
  will be shipped on <shipdate>2001-07-13</shipdate>.
</letter>
```
The xml above can be represented by the following Malina code:
```
letter:
    = Dear Mr.
    name = John Smith
    = "Your order "
    orderid = 1032
    = "will be shipped on "
    shipdate = 2001-07-13
```

The same data structure defined using [inline syntax](#inline-syntax):

```
letter: = "Dear Mr." name = "John Smith" = "Your order " orderid = '1032' = "will be shipped on " shipdate = 2001-07-13
```

The same data described in Malina using [WSA mode](#wsa-mode):

```
letter: ( 
    = "Dear Mr." name = "John Smith" 
    = "Your order " orderid = '1032' 
    = "will be shipped on " shipdate = "2001-07-13" )
```

# Command line tool
The executable file `mlc.exe` is a command line tool that compiles specified files, stores results in the output directory and validate output XML against XML schema if XSD files are listed in the options.
The command line format is:

`mlc [options] [inputFiles]`

Options:
+ `-i=DIR`             Input directory with mlx, mlj and xsd files
+ `-o=DIR`             Output directory
+ `-r`                 Turns on recursive search of files in the input directories

You can specify one or many input directories or files of type .mlx, .mlj and xsd. If neither directories nor files are given, then compiler takes them from the current directory. 

