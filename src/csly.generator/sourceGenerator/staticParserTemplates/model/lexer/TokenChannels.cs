using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace <#NS#>;




public static class Channels
{
    public const int Main = 0;
    public const int WhiteSpaces = 1;
    public const int Comments = 2;
}

public class TokenChannels<IN> : IEnumerable<Token<IN>>  where IN : struct, Enum
{
    
    
    private readonly Dictionary<int, TokenChannel<IN>> _tokenChannels;
    
    private readonly List<Token<IN>> _allTokens;
    
    public List<Token<IN>> AllTokens => _allTokens;

    public IEnumerable<Token<IN>> GetAllExceptWhiteSpaces() => _allTokens.Where(x => !x.IsWhiteSpace);
    
    public List<Token<IN>> MainTokens() => GetChannel(Channels.Main).Tokens.Where(x => x != null).ToList();
     
    

    public TokenChannels()
    {
        _tokenChannels = new Dictionary<int, TokenChannel<IN>>();
    }

    public TokenChannels(List<Token<IN>> allTokens) : this()
    {
        _allTokens = allTokens;
        int i = 0;
        foreach (var token in allTokens)
        {
            token.PositionInTokenFlow = i;
            Add(token);
            i++;
        }
    }

    public IEnumerable<TokenChannel<IN>> GetChannels()
    {
        var channels = new List<TokenChannel<IN>>(_tokenChannels.Count);
        foreach (var kvp in _tokenChannels)
        {
            channels.Add(kvp.Value);
        }
        // Sort by ChannelId
        for (int i = 0; i < channels.Count - 1; i++)
        {
            for (int j = i + 1; j < channels.Count; j++)
            {
                if (channels[j].ChannelId < channels[i].ChannelId)
                {
                    var temp = channels[i];
                    channels[i] = channels[j];
                    channels[j] = temp;
                }
            }
        }
        return channels;
    }
    
    public TokenChannel<IN> GetChannel(int i)
    {
        return _tokenChannels[i];
    }

    public Token<IN> this[int index]
    {
        get => GetToken(index);
    }

    public int Count => MainTokens().Count;

    private Token<IN> GetToken(int index)
    {
        var list = MainTokens();
        return list[index];
    }

    public void Add(Token<IN> token)
    {
        token.TokenChannels = this;
        TokenChannel<IN> channel = null;
        int mx = 0;
        if (_tokenChannels?.Values != null && _tokenChannels.Count > 0)
        {
            foreach (var ch in _tokenChannels.Values)
            {
                if (ch.Count > mx)
                {
                    mx = ch.Count;
                }
            }
        }
        
        if (!TryGet(token.Channel, out channel))
        {
            
            
            channel = new TokenChannel<IN>(token.Channel);
            int shift = 0;
            if (_tokenChannels.Count > 0)
            {
                shift = mx;
            }
            for (int i = 0; i < shift; i++)
            {
                channel.Shift();
            }
            _tokenChannels[token.Channel] = channel;
        }

        int index = mx;
        foreach (var VARIABLE in _tokenChannels.Values)
        {
            for (int i = channel.Count; i < index; i++)
            {
                channel.Shift();
            }

            if (channel.ChannelId == token.Channel)
            {
                channel[index] = token;
            }
        }
        
        
    }


    public bool TryGet(int index, out TokenChannel<IN> channel) => _tokenChannels.TryGetValue(index, out channel);

    public IEnumerator<Token<IN>> GetEnumerator()
    {
        return MainTokens().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return MainTokens().GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var strings = new string[_tokenChannels.Count];
        int idx = 0;
        foreach (var channel in _tokenChannels.Values)
        {
            strings[idx++] = channel.ToString();
        }
        return string.Join("\n", strings);
    }

    public void PreCompute()
    {
        foreach (var channel in _tokenChannels)
        {
            channel.Value.PreCompute();
        }
    }
}