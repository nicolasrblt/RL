import json

from .message import Message


class ObservationMessage(Message):
    """
    Message specifying an environment state
    """
    def __init__(self, agentPostion, agentRotation, velocity, angularVelocity, 
                 redBallPosition, blueBallPosition, greenBallPosition, 
                 grayAreaPosition, orangeAreaPosition, whiteAreaPosition,
                 agentRedBallAngle, agentGrayAreaAngle, agentRedBallDist,
                 agentGrayAreaDist, RedBallGreyAreaDist, agentOutsidePlane,
                 redBallOutsidePlane,terminate, reward, envNum):
        
        self.agentPostion = agentPostion
        self.agentRotation = agentRotation
        
        self.velocity = velocity
        self.angularVelocity = angularVelocity
        
        self.redBallPosition = redBallPosition
        self.blueBallPosition = blueBallPosition
        self.greenBallPosition = greenBallPosition
        
        self.grayAreaPosition = grayAreaPosition
        self.orangeAreaPosition = orangeAreaPosition
        self.whiteAreaPosition = whiteAreaPosition

        self.agentRedBallAngle = agentRedBallAngle
        self.agentGrayAreaAngle = agentGrayAreaAngle
        self.agentRedBallDist = agentRedBallDist
        self.agentGrayAreaDist = agentGrayAreaDist
        self.RedBallGreyAreaDist = RedBallGreyAreaDist
        self.agentOutsidePlane = agentOutsidePlane
        self.redBallOutsidePlane = redBallOutsidePlane
        
        self.terminate = terminate
        self.reward = reward

        self.envNum = envNum
