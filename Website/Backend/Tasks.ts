interface Task {
    id: string,
    title: string,
    description: string,
    functionName: string,
    variables: string[],
    testCases: TestCases[]
}

export interface TestCases
{
    inputs: string[],
    output: string
}

export const allTasks: Task[] = [
    {
        id: "welcome_task_1",
        title: "Task 1: Welcome",
        description: "The function 'SayHi' should return 'Hello World'. Do this by connecting the exiting blocks in front of you.",
        functionName: "SayHi",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "Hello World"
            }
        ]
    },
    // {
    //     id: "welcome_task_2",
    //     title: "Task 1: Welcome Code",
    //     description: "Now, the function 'SayCode' should return 'Hello Code'. Do this by editing the text.",
    //     functionName: "SayCode",
    //     variables: [],
    //     testCases: [
    //         {
    //             inputs: [],
    //             output: "Hello Code"
    //         }
    //     ]
    // },
    {
        id: "math_task_1",
        title: "Task 2: Simple multiplication",
        description: "The function 'SolveMathProblem' should return the value of 2 x 3. Do this by using the arithmetic block and number blocks in front of you.",
        functionName: "SolveMathProblem",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "6"
            }
        ]
    },
    {
        id: "math_task_2",
        title: "Task 3: Sum of two variables",
        description: "The function 'SolveMathProblem' should return the sum of variables 'X' and 'Y'.",
        functionName: "SolveMathProblem",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["-1", "1"],
                output: "0"
            },
            {
                inputs: ["0", "0"],
                output: "0"
            },
            {
                inputs: ["4", "6"],
                output: "10"
            }
        ]
    },
    {
        id: "math_task_3",
        title: "Task 4: Add new blocks",
        description: "The function 'SolveMathProblem' should return the multiplication of the variables 'X' and 'Y'. You need to add the needed blocks from the menu to the left.",
        functionName: "SolveMathProblem",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["-1", "1"],
                output: "-1"
            },
            {
                inputs: ["0", "0"],
                output: "0"
            },
            {
                inputs: ["4", "6"],
                output: "24"
            }
        ]
    },
    {
        id: "age_bug",
        title: "Task 5: Bug in code",
        description: "The function 'IsAllowedPension' should return true if the given age is greater or equal to 62. The code starts out faulty. Can you fix it?",
        functionName: "IsAllowedPension",
        variables: ["age"],
        testCases: [
            {
                inputs: ["-1"],
                output: "false"
            },
            {
                inputs: ["61"],
                output: "false"
            },
            {
                inputs: ["26"],
                output: "false"
            },
            {
                inputs: ["62"],
                output: "true"
            },           
            {
                inputs: ["78"],
                output: "true"
            }
        ]
    },
    {
        id: "match_correct_function_task",
        title: "Task 6: Match number and function",
        description: "The function 'MakeMeTrue' should return true. Do this by putting the correct function call with the correct number. For example, a function that return 6 should be matched with the number 6.",
        functionName: "MakeMeTrue",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "true"
            },

        ]
    },
    {
        id: "order_numbers_task",
        title: "Task 7: Order numbers",
        description: "In the function 'OrderNumbers', move the number blocks such that they are in acceding order. That is, 0 1 2 3 4... Remember to only move the number blocks! You are not allowed to edit the input.",
        functionName: "OrderNumbers",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "26"
            },

        ]
    },
    // {
    //     id: "drive_task",
    //     title: "Task 1: Allowed to drive checker",
    //     description: "Given the age and the drunk state of a person, the function 'canDrive' should return true if the person can drive and false if not.",
    //     functionName: "canDrive",
    //     variables: ["age", "isDrunk"],
    //     testCases: [
    //         {
    //             inputs: ["10", "false"],
    //             output: "false"
    //         },
    //         {
    //             inputs: ["19", "true"],
    //             output: "false"
    //         },
    //         {
    //             inputs: ["20", "false"],
    //             output: "true"
    //         }
    //     ]
    // },
    // {
    //     id: "sum_task",
    //     title: "Task 2: Add numbers",
    //     description: "Given two numbers (x, y) the sum should be returned in the function 'addTwoNumbers'",
    //     functionName: "addTwoNumbers",
    //     variables: ["x", "y"],
    //     testCases: [
    //         {
    //             inputs: ["1", "1"],
    //             output: "2"
    //         },
    //         {
    //             inputs: ["-5", "5"],
    //             output: "0"
    //         },
    //         {
    //             inputs: ["0", "0"],
    //             output: "0"
    //         }
    //     ]
    // },
    {
        id: "area_task",
        title: "Task 8: Calculate Area",
        description: "The function 'CalculateArea' should return the area of a rectangle with the sides 'X' and 'Y'. However, if any of the sides are less than zero, the function should return 0",
        functionName: "CalculateArea",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["-1", "1"],
                output: "0"
            },
            {
                inputs: ["0", "0"],
                output: "0"
            },
            {
                inputs: ["1", "-1"],
                output: "0"
            },
            {
                inputs: ["2", "3"],
                output: "6"
            }
        ]
    },
    {
        id: "create_function_task",
        title: "Task 9: Create Function",
        description: "The function 'CallFunction' should call a new function that you need to create. The new function should be called 'foo' and have the inputs 'a' and 'b'. It should only return true if a > b, otherwise false. Call the function using x as parameter a and y as parameter b.",
        functionName: "CallFunction",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["3", "1"],
                output: "true"
            },
            {
                inputs: ["1", "1"],
                output: "false"
            },
            {
                inputs: ["4", "9"],
                output: "false"
            }
        ]
    },
    {
        id: "all_task_completed",
        title: "Done",
        description: "All tasks are completed!",
        functionName: "NAN",
        variables: [],
        testCases: [
            {
                inputs: ["NAN"],
                output: "nan"
            },
        ]
    },
]

export default Task;