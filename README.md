# Malina    
**MA**rkup **L**anguage **IN**tended for **A**ll
**Reviewed till [Alias](#alias)**
[![Build status](https://ci.appveyor.com/api/projects/status/8ehh57khcupl5nte?svg=true)](https://ci.appveyor.com/project/syntactik/malina)

## Overview
Malina is a people friendly markup language with code reuse features designed to be semantically compatible with XML and JSON formats.

- Define data using clean and intuitive syntax.
- Define aliases and reuse them as code fragments or document templates.
- Compile Malina documents into XML or JSON files.
- Validate Malina documents against XML schema.

Malina uses indents to define document structure similarly to Python and YAML. Inline and white space agnostic syntax is also available if it's needed to "keep it short".

The purpose of the language is:
- expanding audience of people working with data structures.
- improving productivity of people working with XML and JSON .
- creating new use cases by introducing the data oriented markup language with people friendly syntax and code reuse features.

## Language design requirements

Malina is designed with the following initial requirements:
- It must be more people friendly than XML and JSON.
- It must be simpler than YAML.
- It must be symantically compatible with XML and JSON to replace them in editing (there is no intention to replace XML and JSON with Malina in APIs).
- It must have powerfull code reuse features.

## Language design principles

To meet the requirements mentioned above the following language design principles were used:
- Malina uses indents to define document structure (like Python, YAML etc). 
- Inline and white space agnostic syntax is also supported to satisfy needs of advanced users.
- Quotes are not required to define literals. Quoted strings are used in special occasions like inline definitions, strings with special (escaped) symbols, string interpolation.
- Malina provides syntax to support basic primitives of both XML (like namespaces, elements and attributes) and JSON (like collections and lists).
- Malina provides code reuse features similar to code reuse features of modern programming languages in a form of parameterized aliases and interpolated strings.

## Example

```
//Namespace declarations
#ipo = http://www.example.com/myipo 
#xsi = "http://www.w3.org/2001/XMLSchema-instance" //"xsi" is a namespace prefix.

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
				.quantity = 2 //Argument of the alias
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
		quantity = %quantity = 1 //Parameter with default value
		price = 99.95
		ipo.comment = Need this for the holidays!
		shipDate = 2016-12-12				
```
## Language reference

### Text encoding
- Currently the lexer supports **UTF-8**.
- Malina is **case sensitive**.
- Indents are defined by **whitespaces** formed by tab (**ASCII 0x9**, \t) or space (**ASCII 0x20**) symbols.
- Malina supports "Windows" (ASCII **0x0D0A**, \r \n) and "Linux" (ASCII **0x0A**, \n) **new line** symbols. 

### Comments

Malina supports single line comments. The comments start with **//**
```
//The whole line is a comment
name = "John Smith" //The rest of the line is a comment
```

### Name/Value pair
Malina language is built upon the **Name/Value pair** abstraction.
This abstraction consists of 3 parts:
- [Name](#name)
- [Assignment](#assignment)
- [Value](#value)

Any part could be omitted. For example omitted name or value means that the name or value is empty. If the value is omitted then the assignment can be omitted too.

### Name
A name starts with the [Symbol Prefix](#symbol-prefix) followed by the [Identifier](#identifier).

### Symbol Prefix
A **Symbol Prefix** defines type of the language construct. 
If the prefix is omitted then the language construct is [Element](#Element).
This is the list of the prefixes with the corresponding language constructs:
 

| Symbol Prefix        | Type           |
| :-------------: |:-------------|
|       | [Element](#Element) |
| **!**      | [Document](#Document) |
| **!$** | [Alias Definition](#Alias-Definition)      |
| **#**      | [Namespace Declaration](#Namespace-Declaration) or [Scope](#Scope) |
| **@**  | [Attribute](#Attribute)   |
| **$** | [Alias](#Alias) |
| **.** | [Argument](#Argument) |
| **%** | [Parameter](#Parameter) |

### Identifier

In the current version of the language, **IDs** are defined by the same rules as for an XML element name:
- Element names are case-sensitive
- Element names must start with a letter or underscore
- Element names can contain letters, digits, hyphens, underscores, and periods
- Element names cannot contain spaces
- See the full spec for the XML Name gere http://www.w3.org/TR/REC-xml/#NT-Name

>>JSON has different rules for names. In the future, for the sake of the full symantic compatibility with JSON, the support of JSON names will be added by introduction of "quoted IDs". Still, the current implementation covers majority of the use cases because names with spaces or other special symbols are used rarely in the real life scenarios.

#### Compound Identifier
ID with dot(s) is called a **Compound Identifier**. Dots split a compound identifier in several parts.
In the [Element](#element), [Attribute](#attribute) or [Scope](#scope) the **compound identifier** is used to specify [namespace prefix](#namespace-prefix). 
For example, in the following code the atrribute and element have the namespace prefix `ipo`.
```
@ipo.export-code = 1
ipo.name = Robert Smith
```
A Compound Identifier can be used in the all language constructs except a [Namespace Declaration](#namespace-declaration).
[Element](#element), [Attribute](#attribute) or [Scope](#scope) use it to specify namespace prefix. In all other cases, compound identifier is treated as a whole ID.
[Alias definitions](#alias-definition) can be logically structured by using the compound IDs in the same way classes are structured using namespaces in programming languages:
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
>>Code editors can take advantage of compound IDs to provide a better autocomplete.

#### Simple Identifier
The term **Simple Identifier** is used as opposite to the term of [Compound Identifier](#compound-identifier), meaning that the identifier doesn't have any dots in it, thus it is not divided in parts and represents a single word identifier.
In the example below the `name` is a simple identifier:
```
name = John Smith
```

### Value
There are two types of values: **literal** and **object**.

#### Literal
In Malina, **literals** have 4 types: 
- **strings** are the only literals used in XML. In addition to string literals, JSON also uses: 
- **number**, 
- **boolean** and 
- **null literals**. 

The **empty literal** is defined by ommited [value](#value) and represents the **empty string**.

#### Object
**Object** is a set of name/value pairs. A set of name/value pairs is defined in Malina using [Blocks](#Block).

#### Empty Object
The empty objects is a special type of value which, being an [object](#object), doesn't contain any name/value pairs.
The [empty block](#empty-block) represents the **empty object**.

### Block
Block is a syntax construct for defining an [object](#object). 
As mentioned in the section ["language design principles"](#language-design-principles), malina uses indents to define document structure. The consiquent lines of code, having the same indentation, belong to the same block.
>A line with the greater indent still indirectly belongs to the same block because it is actually a part of a nested block.

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
### Empty Block
Block can be empty. In this case it represents the [empty object](#empty-object).
In the example below, the element `items` has the [empty object value](#empty-object).
```
items:
comments = this is the empty order
```

### Assignment
Malina has two types of an assignment operator: **literal assignment** and **object assignment**.

#### Literal Assignment
The **literal assignment** is used when the right side of the assignment is a [literal](#literal). Equality sigh **=** is used for the assignment of a literal:
```
name = John Smith
```

#### Object Assignment
The **object assignment** is used when the right side of the assignment is an [object](#object). Colon `:` is used for the assignment of an object value:
```
address:
    street = 1100 Main St
    city = Prescott
    state = AZ
```
In the example above, the element `address` has an [object value](#object) which consists of three elements: `street`, `city` and `state`.

>>With the two types of assignments, Malina resolves syntax ambiguities and, at the same time, improves readability of code.


### Module (file)
Malina has a notion of a **Module**. 
Module is a container for:
- [Namespace declarations](#namespace-Declaration)
- [Documents](#document)
- [Alias Definitions](#alias-Definition)

A **Module** is physically represented by a file with the extentions **"mlx"** or **"mlj"**.

#### Mlx-module

**Mlx-module** is physically represented by a file with the extentions **"mlx"**. **Mlx-module** contains [documents](#document) symantically representing XML documents. 

#### Mlj-module

**Mlj-modules** is physically represented by a file with the extentions **"mlj"**. **Mlj-module** contains [documents](#document) symantically representing JSON documents.

The differences in the MLX and MLJ symantics are covered later in this document.

### Namespace Declaration
Namespaces can be declared in the [Module](#module), [Document](#document) or [Alias Definition](#alias-definition).
The hash symbol `#` starts the declaration of a namespace followed by a name/value pair, there an [identifier](#identifier) represents an [xml namespace prefix](https://www.w3.org/TR/xml-names11/#NT-Prefix) and a [string literal](#literal) represents an [xml namespace name](https://www.w3.org/TR/xml-names11/#dt-NSName).
For example:
```
#xsi = http://www.w3.org/2001/XMLSchema-instance
#ipo = http://www.example.com/myipo
```

#### Namespace Prefix
A **namespace prefix** is a [simple identifier](#simple-identifier).
The namespace declared in the module is visible in the all documents and alias definitions in this module. The namespace declared in the document and alias definitions is visible only inside the document or alias definition.  The namespace declared in the document or alias definition overrides the namespace with the same namespace prefix declared in the module.

### Document
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
#### Mlx-document
An **Mlx-document** represents the XML file. It must be defined in the [**mlx-module**](#module).
Mlx-document must have only one root element. 

#### Mlj-document
A **Mlj-document** represent then JSON file. It must be defined in [**mlj-modules**](#module).

Because in JSON the literal value represents the valid document, **Mlj-document** name can be also followed by equality `=` and [literal value](#literal).
```
!document = This is a valid json document.
```

### Element
An Element is the most used type of a name/value pair in Malina. It corresponds to XML element and name\value pair in JSON object. 
Element [name](#name) doesn't have a [symbol prefix](#symbol-prefix).
An element name can be a [compound identifier](#compound-identifier). In this case first part of the name is a [namespace prefix](#namespace-prefix). The [namespace prefix](#namespace-prefix) has to be declared in the [module](#module), [document](#document) or [alias definition](#alias-definition). 
```
ipo.name = Robert Smith
```
If the [assignment](#assignment) and [value](#value) are omitted then the element has the [empty object value](#empty-object).

### Attribute
An **attribute** corresponds to an attribute in XML and name\value pair in JSON object.
Generally it should not be used in [mlj-documents](#mlj-document) but it is allowed to do so. In this case it will be treated like the [element](#element) with the same name.
An attribute starts with "at sign", `@`, followed by [identifier](#identifier) that can be [compound](#compound-identifier). 
In the compound identifier, the first part of the name is a namespace prefix. The namespace prefix has to be declared in the [module](#module), [document](#document) or [alias definition](#alias-definition).
The **attribute** can have only the [literal value](#literal). If the [assignment](#assignment) and [value](#value) are omitted then the attribute has the **empty string value**.
```
@orderDate = 2016-12-01
@ipo.export-code = 1
@emptyString = 
@empty
```

### Alias
Malina has еру powerful code reuse feature called **Alias**. Basically, an Alias is a short name for a fragment of code.
An alias starts with a dollar sign `$` followed by a [simple identifier](#simple-identifier) or [compound identifier](#compound-identifier). 
In some cases a [value](#value) and [assignment](#assignment) are omitted in the **alias**. In other cases the **alias**  has the assignment and the value in the form of a [block of arguments](#block-of-arguments) or [default argument](#default-argument).

An alias must be defined in the [Alias Definition](#alias-definition). It is recommended to use [compound identifiers](#compound-identifier) to create tree structure of aliases.
Depending on the code it represents, the alias can be either a [literal alias](#literal-alias) or an [object alias](#object-alias).

#### Literal Alias
A **Literal Alias** represent a [literal](#literal) and can be used in place of the literal value or inside of the [interpolated string](#interpolated-string).
```
// Alias CurrentDate has the simple identifier and the literal value
@orderDate = $CurrentDate

// Alias $customer.firstName has the compound identifier. 
// It is used inside the interpolated string.
@customerGreating = 'Hello $customer.firstName'
```
#### Object Alias

An **Object Alias** represents an object that have only the following types of [name/value pairs](#name-value-pair):

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
### Alias Definition
[Aliases](#alias) are defined in the [module](#module). An **Alias Definition** starts with the exclamation and dollar signs `!$` followed by an [Identifier](#identifier).

#### Object Alias Definition 
#### Literal Alias Definition
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


### Parameter
[Alias Definition](#alias-definition) can have **parameters** and then it's called a **parameterized alias definition**.
A parameter can represent either an [object](#object) or [literal](#literal) value, thus there are two types of parameters: [object parameter](#object-parameter) and [literal parameter](#literal-parameter). 
A parameter starts with percent sign `%` followed by an [Identifier](#identifier). Two or more parameters with the same name are allowed if they have they have the same value type: [object](#object) or [literal](#literal). 

#### Object parameter
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


#### Literal parameter
**Literal parameter** represents a  whole or partial literal value. 
In the example below, the alias definition `!$Address.US.NJ` has 2 literal parameters: `%name` and `%street`. Both parameters represent the whole literal value. The literal parameter `%customerName` defined in the [literal alias definition](#literal-alias-definition) `!$CustomerGreating` represents a part of the literal value. The parameter `%customerName` is used inside of the [interpolated string](#interpolated-string) `'Hello %customerName'`.
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

#### Default Parameter Value
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
In the following example, the parameter `%name` has the default string value "John Smith", the parameter `%street` has default value defined by the alias `$DefaultStreetName` and the parameter `%customerName` has the default value "My Friend".

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
### Argument
The [Alias](#alias) can have **arguments** if its [definition (alias definition)](#alias-definition) has [parameters](#parameter). An argument can represent either [object](#object) or [literal](#literal) value, thus there are two types of arguments: [object argument](#object-argument) and [literal argument](#literal-argument). 
An argument starts with dot `.` followed by an [Identifier](#identifier).
Each argument corresponds to the parameter with the same name. There are several restrictions for arguments:
1. The argument must have the same [value type](#value) as the corresponding parameter.
2. There can't be two or more arguments with the same name.
3. If the parameter dosn't have a default value then the argument with the same name must be in the alias.
4. If the parameter does have a default value then the corresponding argument can be omitted.

All arguments are passed to the alias as an [object value](#object) in the form of a [Block of Arguments](#block-of-arguments). 


#### Block of Arguments
Block of Arguments is a [block](#block) where each [name/value pair](#name-value-pair) is an [argument](#argument). A block of arguments always represents an [object value](#object) of [alias](#alias).

#### Object Argument
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
#### Literal Argument
A **Literal Argument** is used to specify the value of an [literal parameter](#literal-parameter) in the [alias](#alias).
In the following example, the alias `$$Items.Jewelry.Necklaces.Lapis` has the [block of arguments](#block-of-arguments) which consists of 1 **literal arguments**: `.quantity` with the assigned [literal](#literal) value of `2`. 
```
...
	Items:
		$Items.Jewelry.Necklaces.Lapis: 
		    .quantity = 2
...
```
