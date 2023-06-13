import json

from .message import Message


class ControllMessage(Message):
    def __init__(self, moveInput, turnInput, envNum):
        self.moveInput = float(moveInput)  # convert to float to make sure its serializable
        self.turnInput = float(turnInput)  # (np.float32 are not)
        self.envNum = envNum
