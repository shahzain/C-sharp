using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;

namespace CC_without_split
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            
            
            
            //for user help




            input.Text = "main () { \n\n\n }";
        }

        private void Compile_Click(object sender, EventArgs e)

        {

            //output of lexems stored in this textbox

            output.Text = "";

           

            //for storing tockens

            ArrayList finalArray = new ArrayList();
            ArrayList tocken_array = new ArrayList();



            //temporary array to check variables

            ArrayList temporaryArray_forVariableChecking = new ArrayList();



            //array for storage of valid variables

            ArrayList valid_variables = new ArrayList();



            //temporary erray to store lexemes identified

            ArrayList tempArray = new ArrayList();



            //boolean used to check whater if condition was present before else of not
            //true when present

            Boolean is_if = false;

            //for bracket matching

            int bracket_count = 0;

            //input text that is read character by character
            string strinput = input.Text;

            char[] charinput = strinput.ToCharArray();



            //Regular Expression for Variables

            Regex variable_Reg = new Regex(@"^[A-Za-z|_][A-Za-z|0-9]*$");


            //Regular Expression for Constants

            Regex constants_Reg = new Regex(@"^[0-9]+([.][0-9]+)?([e]([+|-])?[0-9]+)?$");


            //Regular Expression for Operators

            Regex operators_Reg = new Regex(@"^[+-/*=]$");


            //Regular Expression for Special_Characters

            Regex Special_Reg = new Regex(@"^[;{}()<>]$");


            //integer regex

            Regex integer = new Regex(@"^[0-9]+$");



            //making lexemes by one by one character read

            for (int itr = 0; itr < charinput.Length; itr++)

            {

                //checking character matches what type

                Match Match_Variable = variable_Reg.Match(charinput[itr] + "");

                Match Match_Constant = constants_Reg.Match(charinput[itr] + "");

                Match Match_Operator = operators_Reg.Match(charinput[itr] + "");

                Match Match_Special = Special_Reg.Match(charinput[itr] + "");



                //if variable is read or constant is read

                if (Match_Variable.Success || Match_Constant.Success)

                {

                    //add to temp array

                    tempArray.Add(charinput[itr]);
                }


                //if space is read

                if (charinput[itr].Equals(' '))

                {

                    // check temp array cout , if not zero, store temp array to final array

                    if (tempArray.Count != 0)
                    {
                        int j = 0;

                        String fin = "";

                        for (; j < tempArray.Count; j++)

                        {
                            fin += tempArray[j];
                        }

                        finalArray.Add(fin);

                        tempArray.Clear();
                    }

                }


                //if char matches operator or special character

                if (Match_Operator.Success || Match_Special.Success)
                {


                    // check temp array cout , if not zero, store temp array to final array

                    if (tempArray.Count != 0)
                    {
                        int j = 0;
                        String fin = "";

                        for (; j < tempArray.Count; j++)
                        {
                            fin += tempArray[j];
                        }

                        finalArray.Add(fin);
                        tempArray.Clear();
                    }

                    finalArray.Add(charinput[itr]);
                }
            }


            if (tempArray.Count != 0)

            {

                String fina = "";

                for (int k = 0; k < tempArray.Count; k++)

                {

                    fina += tempArray[k];

                }

                finalArray.Add(fina);
            }



            //outputing lexems

            //  output.AppendText("Lexemes Are :\n");

            for (int k = 0; k < finalArray.Count; k++)
            {
                output.AppendText(finalArray[k] + "\n");
            }




            //output tockens in form of array

            for (int k = 0; k < finalArray.Count; k++)
            {
                tocken.AppendText(finalArray[k] + "\t");
            }



            // generating lexemes


            for (int k = 0; k < finalArray.Count; k++)
            {

               

                String tocken = finalArray[k].ToString();

                Boolean is_keyword = false;


                if (tocken==")"|| tocken == "(" || tocken == "}" || tocken == "{" || tocken == ";")
                {
                    lexemes_with_attributes.AppendText("<  "+finalArray[k] + "  ,  key symbols  >\n\n");
                }


                if (tocken == "main" || tocken == "for" || tocken == "if" || tocken == "else" || tocken == "print" || tocken == "int")
                {
                    lexemes_with_attributes.AppendText("<  " + finalArray[k] + "  ,  keywords  >\n\n");
                    is_keyword = true;
                }


                Match is_integer = integer.Match(tocken);

                Match is_variable = variable_Reg.Match(tocken);

              

                if (is_integer.Success == true)
                {
                    lexemes_with_attributes.AppendText("<  " + finalArray[k] + "  ,  constants  >\n\n");
                }

                if (is_variable.Success == true && is_keyword==false)
                {
                    lexemes_with_attributes.AppendText("<  " + finalArray[k] + "  ,  identifier  >\n\n");
                }



            }





            //symbol table initialization ros and columns is size


            int rows = 10;
            int columns = 4;

            String[,] Symbol_table = new String[rows, columns];

            

            for (int i = 0; i < columns; i++)
            {


                for (int j = 0; j < rows; j++)
                {

                    try
                    {

                        Symbol_table[i, j] = "";
                    }
                    catch (System.NullReferenceException E)
                    {

                    }

                    catch (System.IndexOutOfRangeException F)
                    {

                    }



                }
                



            }


            //adding a empty string
            Symbol_T.AppendText("");


            //column to be read by parser

            int column_name = 0;




            //checking for main method in code
            /*
              main
              (
              )
              {
                 is identified
            
             */
            int count = finalArray.Count - 1;

            if (count > 3)
            {



                if (!(finalArray[0].ToString() == "main" && finalArray[1].ToString() == "(" && finalArray[2].ToString() == ")" && finalArray[3].ToString() == "{"))
                {
                    errorBox.AppendText("main method could not be identified  \n");

                }


                if (finalArray[count].ToString() != "}")
                {

                    //last bracket check

                    errorBox.AppendText("brackets mismatch \n"); 
                }


            }


            else
            {

                //if main not identified

                errorBox.AppendText("main method could not be identified_  \n");
            }


            //checking for int a=b; and int a =10;  production rule


            temporaryArray_forVariableChecking = finalArray;


            // when every production rule is ok , ok becomes true

            Boolean ok = false;


            //iterate through the lexemes

            for (int k = 0; k < finalArray.Count; k++)
            {


                String s = finalArray[k].ToString();

                //if int is read check production rule for int a=b; and int a =10;  production rule

                if (s == "int")

                {
                    try
                    {
                        //adding values to symbol table

                        //name

                        Symbol_table[column_name, 0] = finalArray[k + 1].ToString();


                        //type

                        Symbol_table[column_name, 1] = s;



                        //value
                        
                        Symbol_table[column_name, 2] = finalArray[k + 3].ToString();


                        //match variable or constant

                        Match is_integer = integer.Match(finalArray[k + 3].ToString());
                        Match is_variable = variable_Reg.Match(finalArray[k + 3].ToString());

                        if (is_integer.Success)
                        {
                            //status ok if production is valid

                            Symbol_table[column_name, 3] = "ok";
                        }


                        //if not integer, check value of that valriable if present, or error

                        if (!is_integer.Success)

                        {

                            int l = k;

                            while (l > 0)

                            {
                                try
                                {
                                    String check_value = finalArray[k + 3].ToString();

                                    is_integer = integer.Match(finalArray[l + 2].ToString());

                                    is_variable = integer.Match(finalArray[l + 2].ToString());
                                    
                                    //if value is integer then production is valid, else not

                                    if (finalArray[l].ToString() == check_value && is_integer.Success == true)
                                    {

                                        //valid production

                                        ok = true;

                                        Symbol_table[column_name, 3] = "ok";

                                    }
                                   

                                    // if not integer check value of that variable, if present , ok , else error

                                    if (!is_integer.Success==true)
                                    {

                                        try
                                        {
                                            int columns_ = columns;

                                            while (columns_ > 0)

                                            {
                                                String Check = Symbol_table[columns_, 3].ToString();
                                                //Symbol_table[column_name, 2] = Symbol_table[columns_, 2].ToString();

                                                if (Check == "ok")
                                                {
                                                    
                                                    ok = true;
                                                    goto All_good;

                                                }
                                            }
                                        }

                                        catch (System.NullReferenceException E) {
                                           
                                        }
                                    }
                                    All_good:
                                    //decrement pointer

                                    l--;
                                }

                                catch (System.ArgumentOutOfRangeException E) { l--; }
                            }





                            //if ok is false display erroe

                            if (ok == false)
                            {

                                //display error
                                
                                errorBox.AppendText("value for " + "< " + finalArray[k + 1].ToString() + " >" + " is not integer or no semicolon between  " + finalArray[k + 3].ToString() + "  \n\n");

                            }

                        }


                        //integer checking over

                       // check semi colon placement

                        if (!finalArray[k + 4].ToString().Equals(";"))
                        {

                            //error if no semicolon

                            errorBox.AppendText("error in semicolon placement after < " + finalArray[k + 3].ToString() + " >\n");

                        }
                    }

                    catch (System.ArgumentOutOfRangeException E)
                    {
                        errorBox.AppendText("error in semicolon placement or type mismatch \n");
                    }



                    //increment column name to store next value in symbol table

                    column_name++;
                }












                /* checking production rule for if statemnt
                 *  if(a>b){
                 *  checking this where a and b are valid and initialized variables
                 *  in symbol table
                 * 
                 */

                try
                {


                    if (s == "if")
                    {

                /* checking production rule for if statemnt
                 *  if(a>b){
                 *  checking this where a and b are valid and initialized variables
                 *  in symbol table
                 * 
                 */

                        if (finalArray[k].ToString() == "if" && (finalArray[k + 1].ToString() == "(" || finalArray[k + 3].ToString() == ">" || finalArray[k + 3].ToString() == "<" || finalArray[k + 3].ToString() == "==" || finalArray[k + 3].ToString() == "!=") && finalArray[k + 5].ToString() == ")" && finalArray[k + 6].ToString() == "{")
                        {

                            //first operand is valid or not

                            Boolean first_operand = false;


                            //second operand is valid or not

                            Boolean second_operand = false;


                            //if textbox of error is empty then proceed

                            if (String.IsNullOrEmpty(errorBox.Text))
                            {

                                //check symbol table to make sure variable used in if are valid

                                for (int i = 0; i < columns - 1; i++)
                                {

                                    if (!(String.IsNullOrEmpty(finalArray[k + 2].ToString())))
                                    {
                                        String check = (Symbol_table[i, 0].ToString());
                                        
                                        if (check == finalArray[k + 2].ToString())
                                        {
                                            

                                            first_operand = true;

                                            //skip others if found

                                            goto next; 
                                        }

                                    }
                                }




                                //check symbol table to make sure variable used in if are valid
                                next:
                                    for (int i = 0; i < columns - 1; i++)
                                    {

                                    //if value is not null then proceed

                                    if (!(String.IsNullOrEmpty(finalArray[k + 4].ToString())))
                                    {
                                        
                                        String check = (Symbol_table[i, 0].ToString());
                                        if (check == finalArray[k + 4].ToString())
                                        {
                                            second_operand = true;

                                            //if operands are true then poceed

                                            if (first_operand == true)
                                            {
                                                is_if = true;

                                                //go if all goes well

                                                goto ok_if;
                                                
                                            }
                                        }

                                    }
                                 }   



                                // if one of operand or both are invalid, display error

                                if (!(second_operand == true || first_operand == true))
                                {
                                    errorBox.AppendText("\n operands in if condition not correct");
                                }

                                // all good
                                else
                                {
                                    is_if = true;
                                    //if syntax true, can proceed
                                }





                            }

                        }



                        //else if if satntax is incorrect
                        else
                        {
                            errorBox.AppendText("\n if condition not correctly written");
                        }


                    }
                }


                catch (System.NullReferenceException)



                {
                    
                    errorBox.AppendText(" \n if condition not correctly written");

                }

                //label which ensures that if is checked and correct

                ok_if:


                //end of if








                //check for print statement


                try
                {
                    
                        
                    if (s == "print")


                    {

                        if (finalArray[k].ToString() == "print" && finalArray[k + 1].ToString() == "(" && finalArray[k + 3].ToString() == ")")

                        {

                            /* boolean used to check whather
                             * variable used in print statement is
                             * valid or not
                             */

                            Boolean in_print_variable = false;


                            //if textbox of error is empty then proceed

                            if (String.IsNullOrEmpty(errorBox.Text))
                            {

                                //check symbol table for validity of variable used in print

                                for (int i = 0; i < columns - 1; i++)
                                {


                                    String check = (Symbol_table[i, 0].ToString());

                                    //if found

                                    if (check == finalArray[k + 2].ToString())

                                    {
                                        //validity is true

                                        in_print_variable = true;
                                    }
                                }


                                //if not true

                                if (!(in_print_variable == true))
                                {

                                    errorBox.AppendText("\n operands in print statement condition not correct");

                                }
                                else
                                {

                                    //print syntax true, can proceed
                                }



                            }
                        }
                    }
                }


                catch (System.NullReferenceException) { }


                // checking for else production rule

                try
                {

                    // checking if if was present before else statement

                    if (s == "else" && is_if==true)
                    {

                        // checking production syntax

                        if (finalArray[k].ToString() == "else" && (finalArray[k + 1].ToString() == "{" ))
                        {
                            if (String.IsNullOrEmpty(errorBox.Text))
                            {
                                //all good
                            }

                        }


                        //else if if satntax is incorrect

                        else
                        {
                            errorBox.AppendText("\n else condition not correctly written");
                        }
                    }


                    //else written before if condition , generate error

                    if (s == "else" && is_if == false)
                    {
                        
                            errorBox.AppendText("\n else condition written without any if");
                        
                    }

                }

                catch (System.NullReferenceException) { }


                
                //brackets matching
                //++ for { and -- for }
                //end should be zero

                if (s.Equals("{"))
                {
                    bracket_count++;
                }
                if (s.Equals("}"))
                {
                    bracket_count--;
                }



            }

            //if not zero , generate error

            if (!(bracket_count == 0))
            {
                errorBox.AppendText("\n bracket mismatch\n");
            }







            for (int i = 0; i < columns; i++)
            {

                if (Symbol_table[i, 2] != null)
                {
                    if (variable_Reg.Match(Symbol_table[i, 2]).Success)
                    {
                        for (int j = i; j >= 0; j--) {
                            if (Symbol_table[i, 2].Equals(Symbol_table[j, 0])) {
                                Symbol_table[i, 2] = Symbol_table[j, 2]; 
                            }
                        }
                    }
                }
               



            }
            //printing symbol table

            for (int i = 0; i < columns; i++)
            {


                for (int j = 0; j < rows; j++)
                {

                    try
                    {
                        if (!(Symbol_table[i, j].Equals(null)))
                            Symbol_T.AppendText(Symbol_table[i, j]+"\t");
                    }
                    catch (System.NullReferenceException E)
                    {
                        
                    }

                    catch (System.IndexOutOfRangeException F)
                    {

                    }



                }
                Symbol_T.AppendText("\n");
                
               

            }



            //checking symbol table

            for (int i = 0; i < columns; i++)
            {

                int j = 3;
                

                    try

                    {

                    

                        if (!(Symbol_table[i, j].Equals("ok")))
                        {
                            

                        if(Symbol_table[i, j - 3].ToString() != "")
                        {

                            errorBox.AppendText("\n value for "+ Symbol_table[i, j - 3].ToString() +" is not valid \n");
                            //tocken.Text = "";
                            //Symbol_T.Text = "";
                            //output.Text = "";
                            //lexemes_with_attributes.Text = "";
                        }

                           

                        }

                    

                }
                
                       
                    catch (System.NullReferenceException E)
                    {
                    
                    }

                    catch (System.IndexOutOfRangeException F)
                    {

                    }

                



            }


            if (!String.IsNullOrEmpty(errorBox.Text))
            {
              


            }
        }



        
        //if textbox of error is empty then proceed

            

            

        //ignore it

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //action listener for clear botton

        private void button1_Click(object sender, EventArgs e)
        {

            input.Text = "main () { \n\n\n }";
            tocken.Text = "";
            Symbol_T.Text = "";
            errorBox.Text = "";
            output.Text = "";
            lexemes_with_attributes.Text = "";

        }
    }
}