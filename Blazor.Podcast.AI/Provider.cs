using Microsoft.Extensions.AI;

namespace Blazor.Podcast.AI;

public class Provider(IChatClient chat)
{
    // System Prompt
    private const string system = @"
    You are a friendly and useful assistant that will help with podcast planning
    that is based on answers to questions, always state detailed opinions on 
    anything asked of you then suggest title and short description for 
    the podcast, segments for each episode, first five episode ideas, 
    ideas to make it unique and generate a script for a trailer.
    Only use simple html and no markdown to format responses";

    // Cancellation
    private CancellationTokenSource? cancel;

    // Properties
    public string Title { get; } = "Blazor Podcast AI";

    public string Label { get; } = "Optionally refine with details or questions";

    public Dictionary<string, List<string>> Questions { get; } = new()
    {       
        { "Podcast is about", [string.Empty] },
        { "Host of podcast is", [string.Empty] },
        { "Listener of podcast is", [string.Empty] },
        { "Format of podcast is", 
        ["Solo", "Interview", "Cohosted", "Roundtable", "Audiobook"] },
        { "Purpose of podcast is", 
        ["Community", "Discussion", "Education", "Experience", "Entertainment"] }
    };

    public List<ChatMessage> Messages { get; set; } = [new(ChatRole.System, system)];

    public bool IsQuestions { get; set; } = true;

    public bool IsGenerating { get; set; } = false;

    // Send Method
    public async Task Send(string message)
    {
        cancel = new();
        IsGenerating = true;
        IsQuestions = false;
                
        Messages.Add(new ChatMessage(ChatRole.User, message)); 
                
        var response = await chat.GetResponseAsync([.. Messages], null, cancel.Token);
        var assistant = new TextContent(response.Text);

        Messages.Add(new ChatMessage(ChatRole.Assistant, [assistant]));

        IsGenerating = false;
    }

    // Cancel & New Methods
    public void Cancel()
    {            
        cancel?.Cancel();
        IsGenerating = false;
    }

    public void New()
    {
        Cancel();
        IsQuestions = true;
        Messages = [new(ChatRole.System, system)];
    }
}