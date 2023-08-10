import json


class Message:
    """
    A base class representing all messages exchanged with the Unity side of this framework

    This class should be extended to create custom messages
    """
    @classmethod
    def from_json(cls, json_string):
        """
        Build a message from its JSON serialized representation
        """
        message_dict = json.loads(json_string)
        return cls.from_dict(message_dict)

    @classmethod   
    def from_dict(cls, message_dict):
        """
        Build a message from its JSON deserialized representation
        """
        if not isinstance(message_dict, dict):
            raise ValueError(f"malformed JSON, cant convert {message_dict} to {cls}")
        return cls(**message_dict)

    def to_json(self):
        """
        Serialize message to its JSON representation
        """
        return json.dumps(self, cls=MessageEncoder)
    
    def to_dict(self):
        """
        Serialize message to its dict representation
        """
        return self.__dict__
    
    def __eq__(self, __value: object) -> bool:
        if not isinstance(__value, Message):
            return False
        return self.to_dict() == __value.to_dict()
    
    def __string__(self):
        return f"{self.__class__.__name__}({self.__dict__})"
    __repr__ = __string__



class MessageEncoder(json.JSONEncoder):
    """
    Utility class to serialize nested messages
    """
    def default(self, o):
        if isinstance(o, Message):
            return o.to_dict()
