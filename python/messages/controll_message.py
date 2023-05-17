import json

class ControllMessage:
    def __init__(self, moveInput, turnInput):
        self.moveInput = float(moveInput)  # convert to float to make sure its serializable
        self.turnInput = float(turnInput)  # (np.float32 are not)
        
    @staticmethod
    def from_json(json_string):
        message_dict = json.loads(json_string)
        return ControllMessage(message_dict["moveInput"], message_dict["turnInput"])

    def to_json(self):
        return json.dumps(self.__dict__)
