# Spring EL 表达式是什么？
Spring3中引入了Spring表达式语言—SpringEL,SpEL是一种强大,简洁的装配Bean的方式,他可以通过运行期间执行的表达式将值装配到我们的属性或构造函数当中,更可以调用C#中提供的静态常量,获取外部json xml文件中的的配置值

# 为什么要使用SpringEL?
可以方便的注入 外部配置文件到 类的构造方法，属性或者 字段，支持注入容器里面的对象的某个属性值，还可以调用对象的方法，功能非常的强大，请看官方文档的例子或者下面我的单元测试例子

 [SpringEL与Autofac容器相结合框架](https://github.com/yuzd/Autofac.Annotation)

**el表达式灵活使用能极大提高代码的活性**

# SpringEL 如何使用

NUGET地址：
Install-Package Spring.EL

该项目是从spring.core项目抽取出来的，因为我只想用 EL 功能，其他的功能我都去掉了。

```csharp 

外部暴露的对象有下面几个重要对象

- ExpressionEvaluator 提供了很关键的几个静态方法

public static object GetValue(object root, string expression);

public static object GetValue(object root, string expression, IDictionary variables)

public static void SetValue(object root, string expression, object newValue)

public static void SetValue(object root, string expression, IDictionary variables, object newValue)

```

 [Spring官方文档](https://www.springframework.net/doc-latest/reference/html/expressions.html)


## 对象的某个属性获取和重新赋值
![image](https://images4.c-ctrip.com/target/zb051900000162lqtB787.png)
![image](https://images4.c-ctrip.com/target/zb0219000001676eo1273.png)
## 文字表达式

```csharp
        [TestMethod]
        public void TestMethod_02()
        {
            string tonyPizza = (string)ExpressionEvaluator.GetValue(null, "'Tony''s Pizza'"); // evals to "Tony's Pizza"

            string helloWorld = (string)ExpressionEvaluator.GetValue(null, "'Hello World'"); // evals to "Hello World"

            double avogadrosNumber = (double)ExpressionEvaluator.GetValue(null, "6.0221415E+23");

            int maxValue = (int)ExpressionEvaluator.GetValue(null, "0x7FFFFFFF");  // evals to 2147483647

            DateTime birthday = (DateTime)ExpressionEvaluator.GetValue(null, "date('1974/08/24')");

            DateTime exactBirthday = (DateTime)ExpressionEvaluator.GetValue(null, " date('19740824T131030', 'yyyyMMddTHHmmss')");

            bool trueValue = (bool)ExpressionEvaluator.GetValue(null, "true");

            object nullValue = ExpressionEvaluator.GetValue(null, "null");
        }

```


## Properties, Arrays, Lists, Dictionaries, Indexers

```csharp

        [TestMethod]
        public void TestMethod_03()
        {
            Inventor tesla = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");

            tesla.PlaceOfBirth.City = "Smiljan";
            string invention = (string)ExpressionEvaluator.GetValue(tesla, "Inventions[2]"); // "Induction motor"

            string name = (string)ExpressionEvaluator.GetValue(tesla, "Members[0].Name"); // "Nikola Tesla"


            string invention1 = (string)ExpressionEvaluator.GetValue(tesla, "Members[0].Inventions[2]"); // "Wireless communication"
            
            
            Bar bar = new Bar();

            int val = (int)ExpressionEvaluator.GetValue(bar, "[1]"); // evaluated to 2

            ExpressionEvaluator.SetValue(bar, "[1]", 3);  // set value to 3
            
        }
        
```


## Defining Arrays, Lists and Dictionaries Inline

```csharp
        [TestMethod]
        public void TestMethod_05()
        {

            var val = ExpressionEvaluator.GetValue(null, "{1, 2, 3, 4, 5}");
            var val2 = ExpressionEvaluator.GetValue(null, "{'abc', 'xyz'}");
            var val21 = ExpressionEvaluator.GetValue(null, "new int[] {1, 2, 3, 4, 5}");
            var val22 = ExpressionEvaluator.GetValue(null, "#{'key1' : 'Value 1', 'today' : DateTime.Today}");
            var val221 = ExpressionEvaluator.GetValue(null, "#{1 : 'January', 2 : 'February', 3 : 'March'}");

        }

```

## Methods

```csharp

        [TestMethod]
        public void TestMethod_06()
        {
            Inventor tesla = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            //string literal
            char[] chars = (char[])ExpressionEvaluator.GetValue(null, "'test'.ToCharArray(1, 2)");  // 't','e'

            //date literal
            int year = (int)ExpressionEvaluator.GetValue(null, "date('1974/08/24').AddYears(31).Year"); // 2005

            // object usage, calculate age of tesla navigating from the IEEE society.

            int age = (int)ExpressionEvaluator.GetValue(tesla, "Members[0].GetAge(date('2005-01-01'))"); // 149 (eww..a big anniversary is coming up ;)

        }
        
```

## Relational operators

```csharp

        [TestMethod]
        public void TestMethod_07()
        {
            var a1 = ExpressionEvaluator.GetValue(null, "2 == 2");  // true

            var a12 = ExpressionEvaluator.GetValue(null, "date('1974-08-24') != DateTime.Today");  // true

            var a13 = ExpressionEvaluator.GetValue(null, "2 < -5.0"); // false

            var a14 = ExpressionEvaluator.GetValue(null, "DateTime.Today <= date('1974-08-24')"); // false

            var a15 = ExpressionEvaluator.GetValue(null, "'Test' >= 'test'"); // true


            a1 = ExpressionEvaluator.GetValue(null, "3 in {1, 2, 3, 4, 5}");  // true

            a1 = ExpressionEvaluator.GetValue(null, "'Abc' like '[A-Z]b*'");  // true

            a1 = ExpressionEvaluator.GetValue(null, "'Abc' like '?'");  // false

            a1 = ExpressionEvaluator.GetValue(null, "1 between {1, 5}");  // true

            a1 = ExpressionEvaluator.GetValue(null, "'efg' between {'abc', 'xyz'}");  // true

            a1 = ExpressionEvaluator.GetValue(null, "'xyz' is int");  // false

            a1 = ExpressionEvaluator.GetValue(null, "{1, 2, 3, 4, 5} is ArrayList");  // true

            a1 = ExpressionEvaluator.GetValue(null, "'5.0067' matches '^-?\\d+(\\.\\d{2})?$'");  // false
            a1 = ExpressionEvaluator.GetValue(null, @"'5.00' matches '^-?\d+(\.\d{2})?$'"); // true
        }
        
```

## Logical operators


```csharp

        [TestMethod]
        public void TestMethod_10()
        {
            Inventor ieee = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            // AND
            bool falseValue = (bool)ExpressionEvaluator.GetValue(null, "true and false"); //false

            string expression = @"IsMember('Nikola Tesla') and IsMember('Mihajlo Pupin')";
            bool trueValue = (bool)ExpressionEvaluator.GetValue(ieee, expression);  //true

            // OR
            trueValue = (bool)ExpressionEvaluator.GetValue(null, "true or false");  //true

            expression = @"IsMember('Nikola Tesla') or IsMember('Albert Einstien')";
            trueValue = (bool)ExpressionEvaluator.GetValue(ieee, expression); // true

            // NOT
            falseValue = (bool)ExpressionEvaluator.GetValue(null, "!true");

            // AND and NOT
            expression = @"IsMember('Nikola Tesla') and !IsMember('Mihajlo Pupin')";
            falseValue = (bool)ExpressionEvaluator.GetValue(ieee, expression);

        }

```


## Bitwise operators

```csharp
        [TestMethod]
        public void TestMethod_11()
        {
            // AND
            int result = (int)ExpressionEvaluator.GetValue(null, "1 and 3"); // 1 & 3

            // OR
            result = (int)ExpressionEvaluator.GetValue(null, "1 or 3");  // 1 | 3

            // XOR
            result = (int)ExpressionEvaluator.GetValue(null, "1 xor 3");  // 1 ^ 3

            // NOT
            result = (int)ExpressionEvaluator.GetValue(null, "!1"); // ~1

        }
```

## Mathematical operators

```csharp

        [TestMethod]
        public void TestMethod_12()
        {
            // Addition
            int two = (int)ExpressionEvaluator.GetValue(null, "1 + 1"); // 2

            String testString = (String)ExpressionEvaluator.GetValue(null, "'test' + ' ' + 'string'"); //'test string'

            DateTime dt = (DateTime)ExpressionEvaluator.GetValue(null, "date('1974-08-24') + 5"); // 8/29/1974

            // Subtraction

            int four = (int)ExpressionEvaluator.GetValue(null, "1 - -3"); //4

            Decimal dec = (Decimal)ExpressionEvaluator.GetValue(null, "1000.00m - 1e4"); // 9000.00

            TimeSpan ts = (TimeSpan)ExpressionEvaluator.GetValue(null, "date('2004-08-14') - date('1974-08-24')"); //10948.00:00:00

            // Multiplication

            int six = (int)ExpressionEvaluator.GetValue(null, "-2 * -3"); // 6

            double twentyFour = (double)ExpressionEvaluator.GetValue(null, "2.0 * 3e0 * 4"); // 24

            // Division

            int minusTwo = (int)ExpressionEvaluator.GetValue(null, "6 / -3"); // -2

            var one = ExpressionEvaluator.GetValue(null, "8.0 / 4e0 / 2"); // 1

            // Modulus

            int three = (int)ExpressionEvaluator.GetValue(null, "7 % 4"); // 3

            one = ExpressionEvaluator.GetValue(null, "8.0 % 5e0 % 2"); // 1

            // Exponent

            var sixteen = ExpressionEvaluator.GetValue(null, "-2 ^ 4"); // 16

            // Operator precedence

            var minusFortyFive = ExpressionEvaluator.GetValue(null, "1+2-3*8^2/2/2"); // -45

        }

```


## Assignment

```csharp
        [TestMethod]
        public void TestMethod_13()
        {
            Inventor inventor = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");

            String aleks = (String)ExpressionEvaluator.GetValue(inventor, "Name = 'Aleksandar Seovic'");

            DateTime dt = (DateTime)ExpressionEvaluator.GetValue(inventor, "DOB = date('1974-08-24')");
        }
```


## Expression lists


```csharp

        [TestMethod]
        public void TestMethod_14()
        {
            Inventor ieee = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");

            String pupin = (String)ExpressionEvaluator.GetValue(ieee.Members, "([1].Place.City = 'Beograd'; [1].Place.Name = 'Serbia'; [1].Name)");
        }
```


## Types

```csharp
        
        [TestMethod]
        public void TestMethod_15()
        {
            var a1 = ExpressionEvaluator.GetValue(null, "1 is int");

            var a2 = ExpressionEvaluator.GetValue(null, "DateTime.Today");
            var a3 = ExpressionEvaluator.GetValue(null, "new string[] {'abc', 'efg'}");

            Inventor tesla = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            Type dateType = (Type)ExpressionEvaluator.GetValue(null, "T(System.DateTime)");

            Type evalType =
                (Type)ExpressionEvaluator.GetValue(null, "T(Spring.Expressions.ExpressionEvaluator, Spring.EL)");

            bool trueValue = (bool)ExpressionEvaluator.GetValue(tesla, "T(System.DateTime) == DOB.GetType()");

        }

```

## Type Registration & Constructors

```csharp
        [TestMethod]
        public void TestMethod_17()
        {
            Inventor2 tesla = new Inventor2("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            DateTime dt = (DateTime)ExpressionEvaluator.GetValue(null, "new DateTime(1974, 8, 24)");

            // Register Inventor type then create new inventor instance within Add method inside an expression list.
            // Then return the new count of the Members collection.

            TypeRegistry.RegisterType(typeof(PlaceOfBirth));
            int three = (int)ExpressionEvaluator.GetValue(tesla.Members, "( Add(new PlaceOfBirth('Aleksandar Seovic')); Count)");

            TypeRegistry.RegisterType(typeof(Inventor));
            Inventor aleks = (Inventor)ExpressionEvaluator.GetValue(null, "new Inventor('Aleksandar Seovic', date('1974-08-24'), 'Serbian', Inventions = new string[]{'SPELL'})");
        }

```

## Variables

```csharp

        [TestMethod]
        public void TestMethod_19()
        {
            Inventor tesla = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            Dictionary<string, object> vars = new Dictionary<string, object>();
            vars["newName"] = "Mike Tesla2";
            var a = ExpressionEvaluator.GetValue(tesla, "Name = #newName", vars);


            ExpressionEvaluator.GetValue(tesla, "( #oldName = Name; Name = 'Nikola Tesla' )", vars);
            String oldName = (String)vars["oldName"]; // Mike Tesla


            vars["prez"] = "president";
            Inventor2 pupin = (Inventor2)ExpressionEvaluator.GetValue(tesla, "Officers[#prez]", vars);

        }
        
```

## The '#this' and '#root' variables

```csharp
        [TestMethod]
        public void TestMethod_20()
        {
            Inventor ieee = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            // sets the name of the president and returns its instance
            var a1 = ExpressionEvaluator.GetValue(ieee, "Officers['president'].( #this.Name = 'Nikola Tesla'; #this )");


            var a2 = ExpressionEvaluator.GetValue(ieee, "Officers['president'].( #root.Officers.Remove('president'); #this )");
        }
```


## Ternary Operator (If-Then-Else)
```csharp

        [TestMethod]
        public void TestMethod_21()
        {
            String aTrueString = (String)ExpressionEvaluator.GetValue(null, "false ? 'trueExp' : 'falseExp'"); // trueExp
        }
        
        [TestMethod]
        public void TestMethod_22()
        {
            Inventor ieee = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            ExpressionEvaluator.SetValue(ieee, "Name", "IEEE");
            Dictionary<string, object> vars = new Dictionary<string, object>();
            vars["queryName"] = "Nikola Tesla";

            string expression = @"IsMember(#queryName)
                      ? #queryName + ' is a member of the ' + Name + ' Society'
                      : #queryName + ' is not a member of the ' + Name + ' Society'";

            String queryResultString = (String)ExpressionEvaluator.GetValue(ieee, expression, vars);
        }
        
```

## List Projection and Selection

```csharp
        [TestMethod]
        public void TestMethod_23()
        {
            Inventor ieee = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), "Serbian");
            IList placesOfBirth = (IList)ExpressionEvaluator.GetValue(ieee, "Members.!{Name}"); // { 'Smiljan', 'Idvor' }


            IList serbianInventors = (IList)ExpressionEvaluator.GetValue(ieee, "Members.?{Name == '1'}"); // { tesla, pupin }


            IList sonarInventors = (IList)ExpressionEvaluator.GetValue(ieee, "Members.?{'1' in Inventions}"); // { pupin }


            IList sonarInventorsNames = (IList)ExpressionEvaluator.GetValue(ieee, "Members.?{'1' in Inventions}.!{Name}"); // { 'Mihajlo Pupin' }


            var a1 = ExpressionEvaluator.GetValue(ieee, "Members.^{Name == '1'}.Name");  // 'Nikola Tesla'
            var a3 = ExpressionEvaluator.GetValue(ieee, "Members.${Name == '3'}.Name"); // 'Mihajlo Pupin'

        }

```

## Collection Processors and Aggregators

```csharp
        [TestMethod]
        public void TestMethod_24()
        {
            var a1 = ExpressionEvaluator.GetValue(null, "{1, 5, -3}.count()");  // 3
            var a2 = ExpressionEvaluator.GetValue(null, "count()"); // 0
            a2 = ExpressionEvaluator.GetValue(null, "{1, 5, -3, 10}.sum()");  // 13 (int)
            a2 = ExpressionEvaluator.GetValue(null, "{5, 5.8, 12.2, 1}.sum()"); // 24.0 (double)
            a2 = ExpressionEvaluator.GetValue(null, "{1, 5, -4, 10}.average()");  // 3
            a2 = ExpressionEvaluator.GetValue(null, "{1, 5, -2, 10}.average()"); // 3.5
            a2 = ExpressionEvaluator.GetValue(null, "{1, 5, -3, 10}.min()");  // -3
            a2 = ExpressionEvaluator.GetValue(null, "{'abc', 'efg', 'xyz'}.min()"); // 'abc'

            a2 = ExpressionEvaluator.GetValue(null, "{1, 5, -3, 10}.max()");  // 10
            a2 = ExpressionEvaluator.GetValue(null, "{'abc', 'efg', 'xyz'}.max()"); // 'xyz'


            a2 = ExpressionEvaluator.GetValue(null, "{ 'abc', 'xyz', null, 'abc', 'def', null}.nonNull()");  // { 'abc', 'xyz', 'abc', 'def' }
            a2 = ExpressionEvaluator.GetValue(null,
                "{ 'abc', 'xyz', null, 'abc', 'def', null}.nonNull().distinct().sort()"); // { 'abc', 'def', 'xyz' }

            a2 = ExpressionEvaluator.GetValue(null, "{ 'abc', 'xyz', 'abc', 'def', null, 'def' }.distinct(true).sort()"); // { null, 'abc', 'def', 'xyz' }
            a2 = ExpressionEvaluator.GetValue(null,
                "{ 'abc', 'xyz', 'abc', 'def', null, 'def' }.distinct(false).sort()"); // { 'abc', 'def', 'xyz' }
        }
```


## DI Object References

```csharp
        [TestMethod]
        public void TestMethod_26()
        {
            Dictionary<string, object> vars = new Dictionary<string, object>();
            vars["_sprint_context_resove_"] = new SprintContextResove((name) =>
            {
                if (name == "Inventor") return new Inventor("a", DateTime.Now, "d");
                return new Inventor2("a", DateTime.Now, "d");
            });

            var IsMember = ExpressionEvaluator.GetValue(null, "@(Inventor).IsMember('Nikola Tesla1')", vars);

        }
```


