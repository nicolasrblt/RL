import json

from .message import Message


class ControllMessage(Message):
    """
    Message specifying action to perform for a step
    """
    def __init__(self, moveInput, turnInput, envNum):
        self.moveInput = float(moveInput)  # convert to float to make sure its serializable
        self.turnInput = float(turnInput)  # (np.float32 are not)
        self.envNum = envNum
