using DataDrivenBot.Admin.Models;
using DataDrivenBot.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DataDrivenBot.Admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int conversationID)
        {
            Diagram model = GetDiagramModel(conversationID);

            return View(model);
        }

        public ActionResult Save(string jsonDiagram)
        {
            dynamic jsonModel = JsonConvert.DeserializeObject(jsonDiagram);

            List<dynamic> nodes = jsonModel.nodeDataArray.ToObject<List<dynamic>>();
            List<dynamic> links = jsonModel.linkDataArray.ToObject<List<dynamic>>();

            ConversationDataModel db = new ConversationDataModel();
            ConversationTemplate template = new ConversationTemplate();

            db.ConversationTemplates.Add(template);
            db.SaveChanges();

            foreach (dynamic node in nodes)
            {
                Step step = AddStep(node);
                template.Steps.Add(step);

                db.SaveChanges();

                node.stepID = step.ID;
            }
             
            foreach (dynamic link in links)
            {
                if (link.answer != null)
                    SaveResponseOption(link, nodes);
                else
                    SaveNextStep(link, nodes);

            }

            db.SaveChanges();

            return View("Index", GetDiagramModel(template.ID));
        }

        private Step AddStep(dynamic node)
        {
            string type = node.type;

            switch(type)
            {
                case "Display":
                    Display display = new Display();
                    display.Title = node.title;
                    display.Text = node.text;
                    display.StartingStep = node.startingStep;
                    return display;

                case "ServiceCall":
                    ServiceCall serviceCall = new ServiceCall();
                    serviceCall.Title = node.title;
                    serviceCall.URL = node.serviceUrl;
                    serviceCall.Headers = node.headers;
                    serviceCall.Method = node.method;
                    serviceCall.StartingStep = node.startingStep;
                    return serviceCall;

                case "Question":
                    Question question = new Question();
                    question.Title = node.title;
                    question.Prompt = node.prompt;
                    question.RetryPrompt = node.prompt;
                    question.ResponseTypeID = 2;
                    question.StartingStep = node.startingStep;
                    return question;
                case "Condition":
                    Condition condition = new Condition();
                    condition.Expression = node.expression;
                    return condition;                
                default:
                    return null;
            }
        }
        

        private void SaveResponseOption(dynamic link, List<dynamic> nodes)
        {
            ConversationDataModel db = new ConversationDataModel();
            int to = (int)link.to.Value;
            int from = (int)link.from.Value;
            string answer = link.answer.Value;

            int fromNodeStepID = nodes.First(n => n.key == from).stepID;
            Question fromQuestion = db.Steps.Find(fromNodeStepID) as Question;

            int toNodeStepID = nodes.First(n => n.key == to).stepID;
            Step toStep = db.Steps.Find(toNodeStepID);

            ResponseOption option = new ResponseOption();
            option.Text = answer;
            option.NextStepID = toStep.ID;
            option.StepID = fromQuestion.ID;

            db.ResponseOptions.Add(option);
            db.SaveChanges();
        }

        private void SaveNextStep(dynamic link, List<dynamic> nodes)
        {
            ConversationDataModel db = new ConversationDataModel();
            int to = (int)link.to.Value;
            int from = (int)link.from.Value;

            int fromNodeStepID = nodes.First(n => n.key == from).stepID;
            Step fromStep = db.Steps.Find(fromNodeStepID);

            int toNodeStepID = nodes.First(n => n.key == to).stepID;
            Step toStep = db.Steps.Find(toNodeStepID);

            fromStep.NextStep = toStep;
            db.SaveChanges();
        }

        private Diagram GetDiagramModel(int conversationID)
        {
            ConversationDataModel db = new ConversationDataModel();

            ConversationTemplate template = db.ConversationTemplates.First(t => t.ID == conversationID);

            List<dynamic> nodes = new List<dynamic>();
            List<dynamic> links = new List<dynamic>();

            foreach (Step step in template.Steps.ToList())
            {
                dynamic node = new ExpandoObject();

                node.key = step.ID;
                node.stepID = step.ID;
                node.type = step.GetType().BaseType.Name;
                node.title = step.Title;
                node.startingStep = step.StartingStep;
                
                if (step is Question)
                {
                    Question question = (Question)step;

                    node.prompt = question.Prompt;
                    node.retryPrompt = question.RetryPrompt;

                    if (question.ResponseOptions.Any())
                    {
                        foreach (ResponseOption option in question.ResponseOptions)
                        {
                            dynamic link = new ExpandoObject();

                            link.from = step.ID;
                            link.to = option.NextStepID;
                            link.answer = option.Text;

                            links.Add(link);
                        }
                    }
                    else
                    {
                        if (step.NextStepID.HasValue)
                        {
                            dynamic link = new ExpandoObject();
                            link.from = step.ID;
                            link.to = step.NextStepID;
                            links.Add(link);
                        }
                    }

                }
                else if (step is Display)
                {
                    Display display = (Display)step;
                    node.text = display.Text;

                    if (step.NextStepID.HasValue)
                    {
                        dynamic link = new ExpandoObject();
                        link.from = step.ID;
                        link.to = step.NextStepID;
                        links.Add(link);
                    }
                }
                else if (step is ServiceCall)
                {
                    ServiceCall serviceCall = (ServiceCall)step;
                    node.serviceUrl = serviceCall.URL;
                    node.headers = serviceCall.Headers;
                    node.method = serviceCall.Method;

                    if (step.NextStepID.HasValue)
                    {
                        dynamic link = new ExpandoObject();
                        link.from = step.ID;
                        link.to = step.NextStepID;
                        links.Add(link);
                    }
                }
                else
                {
                    Condition condition = (Condition)step;
                    node.expression = condition.Expression;

                    if(condition.NextStepIDWhenFalse.HasValue)
                    {
                        dynamic link = new ExpandoObject();
                        link.from = step.ID;
                        link.fromPort = "L";
                        link.answer = "False";
                        link.to = condition.NextStepIDWhenFalse;
                        link.toPort = "T";
                        links.Add(link);
                    }

                    if(condition.NextStepIDWhenTrue.HasValue)
                    {
                        dynamic link = new ExpandoObject();
                        link.from = step.ID;
                        link.fromPort = "R";
                        link.answer = "True";
                        link.to = condition.NextStepIDWhenTrue;
                        link.toPort = "T";
                        links.Add(link);
                    }
                }
                nodes.Add(node);
            }

            Diagram model = new Diagram();
            model.Links = JsonConvert.SerializeObject(links);
            model.Nodes = JsonConvert.SerializeObject(nodes);

            return model;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}