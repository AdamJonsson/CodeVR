import Task from '../Types/Task';

async function testBlocklyCode(code: string, task: Task): Promise<[boolean, string, string, string]> {
  return new Promise((resolve, reject) => {
    var allTestsPassed = true;
    var inputs: string[] = [];
    var expectedOutput = "";
    var outputFromCurrentlyBlocklyCode: string = "";

    setTimeout(() => {
      reject();
    }, 500);
    
    try {
      for (const testCase of task.testCases) {
        outputFromCurrentlyBlocklyCode = "";
        inputs = testCase.inputs;
        expectedOutput = testCase.output;
  
        eval(
          code +
          `outputFromCurrentlyBlocklyCode = ${task.functionName}(${testCase.inputs.join(",")});`
        );
  
        try {
          outputFromCurrentlyBlocklyCode = outputFromCurrentlyBlocklyCode.toString();
        } catch (error) {}
  
        if (outputFromCurrentlyBlocklyCode !== testCase.output)
        {
          allTestsPassed = false;
          break;
        }
      }
    } catch (error: any) {
      allTestsPassed = false;
      outputFromCurrentlyBlocklyCode = error.message as string;
    }
  
    var readableInputs = ""
    for (let index = 0; index < inputs.length; index++) {
      const input = inputs[index];
      const variable = task.variables[index];
      readableInputs += variable + ": " + input;
      if (index !== inputs.length - 1)
      {
        readableInputs += ", ";
      }
    }
  
    resolve([allTestsPassed, readableInputs, expectedOutput, outputFromCurrentlyBlocklyCode])
    return ;
  });
}

export default testBlocklyCode;