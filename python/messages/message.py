import json


class Message:
    @classmethod
    def from_json(cls, json_string):
        message_dict = json.loads(json_string)
        return cls.from_dict(message_dict)

    @classmethod   
    def from_dict(cls, message_dict):
        if not isinstance(message_dict, dict):
            raise ValueError(f"malformed JSON, cant convert {message_dict} to {cls}")
        return cls(**message_dict)

    def to_json(self):
        return json.dumps(self, cls=MessageEncoder, indent=4)
    
    def to_dict(self):
        return self.__dict__
    
    def __string__(self):
        return f"{self.__class__.__name__}({self.__dict__})"
    __repr__ = __string__



class MessageEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, Message):
            return o.to_dict()
